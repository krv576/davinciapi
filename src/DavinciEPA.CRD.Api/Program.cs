using DavinciEPA.Core.Constants;
using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Exceptions;
using DavinciEPA.Core.Interfaces.Services;
using DavinciEPA.Core.Services;
using DavinciEPA.Fhir.Extensions;
using DavinciEPA.Fhir.Mapping;
using DavinciEPA.Infrastructure.Extensions;
using DavinciEPA.Infrastructure.Logging;
using DavinciEPA.Rules.Extensions;
using DavinciEPA.Security.Authorization;
using DavinciEPA.Security.Extensions;
using DavinciEPA.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDavinciSerilogLogging("DavinciEPA.CRD.Api");

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDavinciSecurity(builder.Configuration);
builder.Services.AddDavinciFhir();
builder.Services.AddDavinciRules();
builder.Services.AddDavinciInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IExceptionResponseMapper, CoreExceptionResponseMapper>();
builder.Services.AddScoped<ICoverageRequirementDiscoveryService, CoverageRequirementDiscoveryService>();

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

var cdsServices = app.MapGroup("/cds-services");

cdsServices.MapGet("/", () => Results.Ok(new
{
    services = new[]
    {
        new CdsServiceDefinitionDto(
            Id: "prior-auth-coverage-requirements",
            Hook: CdsHooksConstants.OrderSelectHook,
            Title: "Prior Authorization Coverage Requirements (Order Select)",
            Description: "Determines whether the order being selected requires prior authorization and identifies any supporting documentation needed.",
            Prefetch: new CdsHooksPrefetchDto(new Dictionary<string, string>
            {
                ["patient"] = "Patient/{{context.patientId}}",
                ["coverage"] = "Coverage?patient={{context.patientId}}&status=active"
            })),
        new CdsServiceDefinitionDto(
            Id: "prior-auth-coverage-requirements",
            Hook: CdsHooksConstants.OrderSignHook,
            Title: "Prior Authorization Coverage Requirements (Order Sign)",
            Description: "Re-confirms prior authorization and documentation requirements before the order is signed.",
            Prefetch: new CdsHooksPrefetchDto(new Dictionary<string, string>
            {
                ["patient"] = "Patient/{{context.patientId}}",
                ["coverage"] = "Coverage?patient={{context.patientId}}&status=active"
            }))
    }
}))
.WithName("GetCdsServices")
.WithOpenApi();

cdsServices.MapPost("/{id}", async (
    string id,
    CdsHooksRequestDto request,
    ICoverageRequirementDiscoveryService discoveryService,
    CoveragePayerExtractor payerExtractor,
    CancellationToken cancellationToken) =>
{
    var prefetch = request.Prefetch ?? new Dictionary<string, string>();
    var orderResourceJson = request.Context.Draft?.Values.FirstOrDefault() ?? "{}";

    var payerId = prefetch.TryGetValue("coverage", out var coverageJson)
        ? payerExtractor.ExtractPayerId(coverageJson) ?? "unknown-payer"
        : "unknown-payer";

    var discoveryRequest = new CoverageRequirementDiscoveryRequestDto(
        request.Hook,
        request.Context.PatientId,
        payerId,
        OrderReference: request.HookInstance,
        orderResourceJson,
        prefetch);

    var result = await discoveryService.DiscoverAsync(discoveryRequest, cancellationToken);

    if (result.IsFailure)
    {
        return Results.Problem(detail: result.Error.Message, statusCode: 400, title: "Coverage requirement discovery failed.");
    }

    var cards = result.Value.Requirements.Select(requirement => new CdsHooksCardDto(
        Summary: requirement.IsMet
            ? "No prior authorization action required."
            : $"Prior authorization required: {requirement.RequirementDescription}",
        Indicator: requirement.IsMet ? CdsHooksConstants.CardIndicatorInfo : CdsHooksConstants.CardIndicatorWarning,
        Detail: requirement.RequirementDescription,
        Source: new CdsHooksSourceDto("DavinciEPA CRD Service", null),
        Links: requirement.DocumentationQuestionnaireCanonicalUrl is null
            ? Array.Empty<CdsHooksLinkDto>()
            : new[]
            {
                new CdsHooksLinkDto(
                    "Complete Required Documentation",
                    $"https://dtr.davinciepa.local/launch?questionnaire={Uri.EscapeDataString(requirement.DocumentationQuestionnaireCanonicalUrl)}",
                    CdsHooksConstants.SmartLinkType)
            }))
        .ToList();

    return Results.Ok(new CdsHooksResponseDto(cards));
})
// .RequireAuthorization(PolicyNames.CdsHooksInvoke)
.AllowAnonymous()
.WithName("InvokeCdsService")
.WithOpenApi();
if (app.Environment.IsDevelopment())

{

    app.UseDeveloperExceptionPage();

}
app.Run();

/// <summary>Entry point partial class exposed so <c>Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory</c> can bootstrap this API in integration tests.</summary>
public partial class Program
{
}
