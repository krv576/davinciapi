using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Enums;
using DavinciEPA.Core.Exceptions;
using DavinciEPA.Core.Interfaces.External;
using DavinciEPA.Core.Interfaces.Services;
using DavinciEPA.Core.Results;
using DavinciEPA.Core.Services;
using DavinciEPA.DTR.Api.Configuration;
using DavinciEPA.DTR.Api.SmartLaunch;
using DavinciEPA.Fhir.Extensions;
using DavinciEPA.Fhir.Mapping;
using DavinciEPA.Infrastructure.Extensions;
using DavinciEPA.Infrastructure.Logging;
using DavinciEPA.Rules.Extensions;
using DavinciEPA.Security.Authorization;
using DavinciEPA.Security.Extensions;
using DavinciEPA.Security.SmartOnFhir;
using DavinciEPA.Shared.Middleware;
using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using DavinciEPA.Core.Constants;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDavinciSerilogLogging("DavinciEPA.DTR.Api");

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDavinciSecurity(builder.Configuration);
builder.Services.AddDavinciFhir();
builder.Services.AddDavinciRules();
builder.Services.AddDavinciInfrastructure(builder.Configuration);

builder.Services.Configure<SmartClientOptions>(builder.Configuration.GetSection(SmartClientOptions.SectionName));
builder.Services.AddSingleton<SmartLaunchStateStore>();

builder.Services.AddSingleton<IExceptionResponseMapper, CoreExceptionResponseMapper>();
builder.Services.AddScoped<IDocumentationRequirementService, DocumentationRequirementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCorrelationId();
app.UseDavinciExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var smartGroup = app.MapGroup("/smart");

smartGroup.MapGet("/launch", (
    string iss,
    string launch,
    IOptions<SmartClientOptions> smartOptions,
    SmartLaunchStateStore stateStore,
    SmartAuthorizationRequestBuilder authorizationRequestBuilder) =>
{
    var codeVerifier = PkceHelper.GenerateCodeVerifier();
    var codeChallenge = PkceHelper.DeriveCodeChallenge(codeVerifier);
    var state = Guid.NewGuid().ToString("N");

    stateStore.Add(state, new SmartLaunchState(codeVerifier, iss, launch));

    // Simplification: assumes the conventional `{iss}/authorize` endpoint rather than performing a full
    // `.well-known/smart-configuration` discovery fetch. See docs/api-design.md for the DTR contract summary.
    var authorizeEndpoint = $"{iss.TrimEnd('/')}/authorize";

    var authorizationUrl = authorizationRequestBuilder.BuildAuthorizationUrl(
        authorizeEndpoint,
        smartOptions.Value.ClientId,
        smartOptions.Value.RedirectUri,
        smartOptions.Value.Scope,
        state,
        launch,
        iss,
        codeChallenge);

    return Results.Redirect(authorizationUrl.ToString());
})
.WithName("SmartLaunch")
.WithOpenApi();

smartGroup.MapGet("/callback", async (
    string code,
    string state,
    IOptions<SmartClientOptions> smartOptions,
    SmartLaunchStateStore stateStore,
    ISmartTokenExchangeClient tokenExchangeClient,
    CancellationToken cancellationToken) =>
{
    if (!stateStore.TryTake(state, out var launchState) || launchState is null)
    {
        return Results.BadRequest(new { message = "Unknown or expired SMART launch state." });
    }

    var tokenEndpoint = $"{launchState.Iss.TrimEnd('/')}/token";

    var token = await tokenExchangeClient.ExchangeAuthorizationCodeAsync(
        tokenEndpoint,
        smartOptions.Value.ClientId,
        smartOptions.Value.RedirectUri,
        code,
        launchState.CodeVerifier,
        cancellationToken);

    return Results.Ok(new
    {
        access_token = token.AccessToken,
        token_type = token.TokenType,
        expires_in = token.ExpiresInSeconds,
        patient = token.Patient,
        scope = token.Scope,
        fhirServer = launchState.Iss
    });
})
.WithName("SmartCallback")
.WithOpenApi();

var documentationGroup = app.MapGroup("/documentation-requirements");

documentationGroup.MapGet("/{id:guid}/questionnaire-package", async (
    Guid id,
    string patient,
    string fhirServer,
    HttpContext httpContext,
    IDocumentationRequirementService documentationService,
    CancellationToken cancellationToken) =>
{
    var authorizationHeader = httpContext.Request.Headers.Authorization.ToString();
    var accessToken = authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
        ? authorizationHeader["Bearer ".Length..]
        : null;

    var result = await documentationService.GetQuestionnairePackageAsync(id, patient, fhirServer, accessToken, cancellationToken);

    if (result.IsFailure)
    {
        return result.Error.Type == ErrorType.NotFound
            ? Results.NotFound(new { result.Error.Message })
            : Results.Problem(detail: result.Error.Message, statusCode: 400);
    }

    return Results.Ok(result.Value);
})
//.RequireAuthorization(PolicyNames.SmartLaunch)
.WithName("GetQuestionnairePackage")
.WithOpenApi();

documentationGroup.MapPost("/{id:guid}/questionnaire-response", async (
    Guid id,
    JsonElement questionnaireResponseJsonElement,
    QuestionnaireResponseExtractor referenceExtractor,
    IDocumentationRequirementService documentationService,
    CancellationToken cancellationToken) =>
{
    var questionnaireResponseJson = questionnaireResponseJsonElement.GetRawText();

    string reference;
    try
    {
        reference = referenceExtractor.ExtractReference(questionnaireResponseJson);
    }
    catch (Exception)
    {
        // Parsing failed — return a controlled 422 instead of allowing a 500
        return Results.UnprocessableEntity(new { message = "QuestionnaireResponse could not be parsed as valid FHIR JSON." });
    }

    var submitDto = new SubmitQuestionnaireResponseDto(id, reference, questionnaireResponseJson);

    var result = await documentationService.SubmitResponseAsync(submitDto, cancellationToken);

    if (result.IsFailure)
    {
        return result.Error.Type == ErrorType.NotFound
            ? Results.NotFound(new { result.Error.Message })
            : Results.UnprocessableEntity(new { result.Error.Message });
    }

    return Results.Ok(result.Value);
})
//.RequireAuthorization(PolicyNames.SmartLaunch)
.WithName("SubmitQuestionnaireResponse")
.WithOpenApi(operation =>
{
    var example = new OpenApiObject
    {
        ["resourceType"] = new OpenApiString("QuestionnaireResponse"),
        ["id"] = new OpenApiString("example-qr-1"),
        ["meta"] = new OpenApiObject
        {
            ["profile"] = new OpenApiArray { new OpenApiString(DaVinciProfiles.DtrQuestionnaireResponse) }
        },
        ["questionnaire"] = new OpenApiString("Questionnaire/advanced-imaging"),
        ["status"] = new OpenApiString("completed"),
        ["subject"] = new OpenApiObject { ["reference"] = new OpenApiString("Patient/1") },
        ["authored"] = new OpenApiString(DateTimeOffset.UtcNow.ToString("o")),
        ["item"] = new OpenApiArray
        {
            new OpenApiObject
            {
                ["linkId"] = new OpenApiString("1"),
                ["answer"] = new OpenApiArray
                {
                    new OpenApiObject { ["valueString"] = new OpenApiString("example response") }
                }
            }
        }
    };

    operation.RequestBody = new OpenApiRequestBody
    {
        Content =
        {
            ["application/json"] = new OpenApiMediaType { Example = example }
        }
    };

    return operation;
});

app.Run();

// The API now accepts raw JSON for QuestionnaireResponse (JsonElement) to avoid double-encoding.

/// <summary>Entry point partial class exposed so <c>Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory</c> can bootstrap this API in integration tests.</summary>
public partial class Program
{
}
