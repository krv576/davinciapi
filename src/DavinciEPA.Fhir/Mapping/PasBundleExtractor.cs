using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Mapping;

/// <summary>Fields extracted from a submitted PAS Bundle needed to drive the Core prior-authorization workflow.</summary>
public sealed record PasSubmissionContext(
    string ClaimReference,
    string PatientIdentifier,
    string PayerId,
    string OrderReference);

/// <summary>Extracts the business fields Core needs from a raw PAS <c>$submit</c> Bundle, keeping Core itself FHIR-agnostic.</summary>
public sealed class PasBundleExtractor
{
    private readonly FhirJsonSerializerService _serializerService;

    public PasBundleExtractor(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public PasSubmissionContext Extract(string bundleJson)
    {
        var bundle = _serializerService.Parse<Bundle>(bundleJson);

        var claim = bundle.Entry
            .Select(entry => entry.Resource)
            .OfType<Claim>()
            .FirstOrDefault()
            ?? throw new InvalidOperationException("The submitted Bundle does not contain a Claim resource.");

        var claimReference = string.IsNullOrWhiteSpace(claim.Id) ? Guid.NewGuid().ToString("N") : claim.Id;
        var patientIdentifier = ExtractLastSegment(claim.Patient?.Reference) ?? "unknown";
        var payerId = ExtractLastSegment(claim.Insurer?.Reference) ?? "unknown";
        var orderReference = claim.Item.FirstOrDefault()?.ProductOrService?.Coding.FirstOrDefault()?.Code ?? "unspecified";

        return new PasSubmissionContext(claimReference, patientIdentifier, payerId, orderReference);
    }

    private static string? ExtractLastSegment(string? reference) =>
        string.IsNullOrWhiteSpace(reference) ? null : reference.Split('/').Last();
}
