using DavinciEPA.Core.Constants;
using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Builders;

/// <summary>Builds a Da Vinci PAS <c>ClaimResponse</c> representing an adjudication decision (approved/denied/pended).</summary>
public sealed class ClaimResponseBuilder : IClaimResponseBuilder
{
    private readonly FhirJsonSerializerService _serializerService;

    private const string TaskIdentifierExtensionUrl = "http://davinciepa.local/fhir/StructureDefinition/pas-task-identifier";

    public ClaimResponseBuilder(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public string BuildClaimResponseJson(
        string claimReference,
        string patientIdentifier,
        string payerId,
        string outcome,
        string? disposition,
        string? taskIdentifier)
    {
        var claimResponse = new ClaimResponse
        {
            Id = Guid.NewGuid().ToString("N"),
            Meta = new Meta { Profile = new[] { DaVinciProfiles.PasClaimResponse } },
            Status = FinancialResourceStatusCodes.Active,
            Type = new CodeableConcept(FhirCodeSystems.ClaimType, "institutional"),
            Use = ClaimUseCode.Preauthorization,
            Patient = new ResourceReference($"Patient/{patientIdentifier}"),
            Created = DateTimeOffset.UtcNow.ToString("O"),
            Insurer = new ResourceReference($"Organization/{payerId}"),
            Request = new ResourceReference(
                claimReference.Contains('/') ? claimReference : $"Claim/{claimReference}"),
            Outcome = MapOutcome(outcome),
            Disposition = disposition
        };

        if (!string.IsNullOrWhiteSpace(taskIdentifier))
        {
            claimResponse.Extension.Add(new Extension(TaskIdentifierExtensionUrl, new FhirString(taskIdentifier)));
        }

        return _serializerService.Serialize(claimResponse);
    }

    private static ClaimProcessingCodes MapOutcome(string outcome) => outcome.ToLowerInvariant() switch
    {
        "complete" => ClaimProcessingCodes.Complete,
        "queued" => ClaimProcessingCodes.Queued,
        "error" => ClaimProcessingCodes.Error,
        "partial" => ClaimProcessingCodes.Partial,
        _ => ClaimProcessingCodes.Complete
    };
}
