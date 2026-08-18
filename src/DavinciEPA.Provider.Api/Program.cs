using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Fhir.Extensions;
using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

const string FhirJsonContentType = "application/fhir+json";
const string SeededPatientId = "1";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDavinciFhir();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var fhirGroup = app.MapGroup("/fhir");

fhirGroup.MapGet("/metadata", (FhirJsonSerializerService serializerService) =>
    Results.Content(serializerService.Serialize(ProviderFhirData.BuildCapabilityStatement()), FhirJsonContentType))
    .WithName("GetCapabilityStatement")
    .WithOpenApi();

fhirGroup.MapGet("/Patient/{id}", (
    string id,
    FhirJsonSerializerService serializerService,
    IOperationOutcomeBuilder operationOutcomeBuilder) =>
{
    if (id != SeededPatientId)
    {
        return Results.Content(
            operationOutcomeBuilder.BuildErrorJson($"Patient '{id}' was not found."),
            FhirJsonContentType,
            statusCode: StatusCodes.Status404NotFound);
    }

    return Results.Content(serializerService.Serialize(ProviderFhirData.BuildPatient()), FhirJsonContentType);
})
.WithName("GetPatient")
.WithOpenApi();

fhirGroup.MapGet("/Condition", (
    string patient,
    FhirJsonSerializerService serializerService,
    IBundleBuilder bundleBuilder) =>
{
    var entries = ProviderFhirData.MatchesSeededPatient(patient)
        ? new[] { serializerService.Serialize(ProviderFhirData.BuildCondition()) }
        : Array.Empty<string>();

    return Results.Content(bundleBuilder.BuildSearchsetBundleJson(entries, entries.Length), FhirJsonContentType);
})
.WithName("SearchCondition")
.WithOpenApi();

fhirGroup.MapGet("/Coverage", (
    string patient,
    FhirJsonSerializerService serializerService,
    IBundleBuilder bundleBuilder) =>
{
    var entries = ProviderFhirData.MatchesSeededPatient(patient)
        ? new[] { serializerService.Serialize(ProviderFhirData.BuildCoverage()) }
        : Array.Empty<string>();

    return Results.Content(bundleBuilder.BuildSearchsetBundleJson(entries, entries.Length), FhirJsonContentType);
})
.WithName("SearchCoverage")
.WithOpenApi();

app.Run();

/// <summary>Hardcoded Provider/EHR reference data for local development and integration testing only.</summary>
internal static class ProviderFhirData
{
    public static bool MatchesSeededPatient(string patientParam) =>
        patientParam is "1" or "Patient/1";

    public static Patient BuildPatient() =>
        new()
        {
            Id = "1",
            Active = true,
            Name = new List<HumanName> { new() { Family = "Example", Given = new[] { "Patient" } } }
        };

    public static Condition BuildCondition() =>
        new()
        {
            Id = "condition-1",
            Subject = new ResourceReference("Patient/1"),
            ClinicalStatus = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/condition-clinical", "active", "Active", text: null!),
            Code = new CodeableConcept("http://hl7.org/fhir/sid/icd-10-cm", "M54.5", "Low back pain", text: null!)
        };

    public static Coverage BuildCoverage() =>
        new()
        {
            Id = "coverage-1",
            Status = FinancialResourceStatusCodes.Active,
            Beneficiary = new ResourceReference("Patient/1"),
            Payor = new List<ResourceReference> { new() { Display = "Aetna" } }
        };

    public static CapabilityStatement BuildCapabilityStatement()
    {
        var statement = new CapabilityStatement
        {
            Status = PublicationStatus.Active,
            Date = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"),
            Kind = CapabilityStatementKind.Instance,
            FhirVersion = FHIRVersion.N4_0_1,
            Format = new[] { "json" }
        };

        var rest = new CapabilityStatement.RestComponent { Mode = CapabilityStatement.RestfulCapabilityMode.Server };
        rest.Resource.Add(BuildResourceComponent("Patient", CapabilityStatement.TypeRestfulInteraction.Read));
        rest.Resource.Add(BuildResourceComponent("Condition", CapabilityStatement.TypeRestfulInteraction.SearchType));
        rest.Resource.Add(BuildResourceComponent("Coverage", CapabilityStatement.TypeRestfulInteraction.SearchType));
        statement.Rest.Add(rest);

        return statement;
    }

    private static CapabilityStatement.ResourceComponent BuildResourceComponent(
        string type,
        CapabilityStatement.TypeRestfulInteraction interaction)
    {
        var component = new CapabilityStatement.ResourceComponent { Type = type };
        component.Interaction.Add(new CapabilityStatement.ResourceInteractionComponent { Code = interaction });
        return component;
    }
}
