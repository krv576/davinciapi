using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Exceptions;
using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Core.Interfaces.Services;
using DavinciEPA.Core.Results;
using DavinciEPA.Core.Services;
using DavinciEPA.Fhir.Extensions;
using DavinciEPA.Fhir.Mapping;
using DavinciEPA.Infrastructure.Extensions;
using DavinciEPA.Infrastructure.Logging;
using DavinciEPA.Rules.Extensions;
using DavinciEPA.Security.Authorization;
using DavinciEPA.Security.Extensions;
using DavinciEPA.Shared.Middleware;

const string FhirJsonContentType = "application/fhir+json";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDavinciSerilogLogging("DavinciEPA.PAS.Api");

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDavinciSecurity(builder.Configuration);
builder.Services.AddDavinciFhir();
builder.Services.AddDavinciRules();
builder.Services.AddDavinciInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IExceptionResponseMapper, CoreExceptionResponseMapper>();
builder.Services.AddScoped<IPriorAuthorizationService, PriorAuthorizationService>();

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

var claimGroup = app.MapGroup("/Claim");

claimGroup.MapPost("/$submit", async (
    HttpContext httpContext,
    PasBundleExtractor bundleExtractor,
    IOperationOutcomeBuilder operationOutcomeBuilder,
    IPriorAuthorizationService priorAuthorizationService,
    CancellationToken cancellationToken) =>
{
    using var reader = new StreamReader(httpContext.Request.Body);
    var bundleJson = await reader.ReadToEndAsync(cancellationToken);

    PasSubmissionContext submissionContext;
    try
    {
        submissionContext = bundleExtractor.Extract(bundleJson);
    }
    catch (Exception ex)
    {
        return Results.Content(
            operationOutcomeBuilder.BuildErrorJson($"Invalid submission Bundle: {ex.Message}"),
            FhirJsonContentType,
            statusCode: StatusCodes.Status400BadRequest);
    }

    var submitDto = new SubmitPriorAuthorizationDto(
        submissionContext.PatientIdentifier,
        submissionContext.PayerId,
        submissionContext.OrderReference,
        submissionContext.ClaimReference,
        bundleJson);

    var result = await priorAuthorizationService.SubmitAsync(submitDto, cancellationToken);

    if (result.IsFailure)
    {
        var statusCode = result.Error.Type == ErrorType.FhirValidation
            ? StatusCodes.Status422UnprocessableEntity
            : StatusCodes.Status400BadRequest;

        return Results.Content(
            operationOutcomeBuilder.BuildErrorJson(result.Error.Message),
            FhirJsonContentType,
            statusCode: statusCode);
    }

    return Results.Content(result.Value.ResponseBundleJson, FhirJsonContentType);
})
//.RequireAuthorization(PolicyNames.SystemClaimWrite)
.WithName("SubmitPriorAuthorization")
.WithOpenApi();

claimGroup.MapGet("/{externalId}/$inquire", async (
    string externalId,
    IPriorAuthorizationService priorAuthorizationService,
    IOperationOutcomeBuilder operationOutcomeBuilder,
    CancellationToken cancellationToken) =>
{
    var result = await priorAuthorizationService.InquireAsync(externalId, cancellationToken);

    if (result.IsFailure)
    {
        var statusCode = result.Error.Type == ErrorType.NotFound
            ? StatusCodes.Status404NotFound
            : StatusCodes.Status400BadRequest;

        return Results.Content(
            operationOutcomeBuilder.BuildErrorJson(result.Error.Message),
            FhirJsonContentType,
            statusCode: statusCode);
    }

    return Results.Content(result.Value.ResponseBundleJson, FhirJsonContentType);
})
//.RequireAuthorization(PolicyNames.SystemClaimRead)
.WithName("InquirePriorAuthorization")
.WithOpenApi();

claimGroup.MapGet("/{externalId}/status", async (
    string externalId,
    IPriorAuthorizationService priorAuthorizationService,
    CancellationToken cancellationToken) =>
{
    var result = await priorAuthorizationService.GetStatusAsync(externalId, cancellationToken);

    return result.IsFailure
        ? Results.NotFound(new { result.Error.Message })
        : Results.Ok(result.Value);
})
//.RequireAuthorization(PolicyNames.SystemClaimRead)
.WithName("GetPriorAuthorizationStatus")
.WithOpenApi();

claimGroup.MapPost("/{externalId}/$cancel", async (
    string externalId,
    IPriorAuthorizationService priorAuthorizationService,
    CancellationToken cancellationToken) =>
{
    var result = await priorAuthorizationService.CancelAsync(externalId, cancellationToken);

    if (result.IsFailure)
    {
        var statusCode = result.Error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return Results.Problem(detail: result.Error.Message, statusCode: statusCode);
    }

    return Results.Ok(result.Value);
})
    //.RequireAuthorization(PolicyNames.SystemClaimWrite)
.WithName("CancelPriorAuthorization")
.WithOpenApi();

app.Run();

/// <summary>Entry point partial class exposed so <c>Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory</c> can bootstrap this API in integration tests.</summary>
public partial class Program
{
}
