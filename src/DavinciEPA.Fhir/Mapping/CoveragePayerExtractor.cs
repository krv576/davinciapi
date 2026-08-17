using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Mapping;

/// <summary>Extracts the payer/insurer identifier from a <c>Coverage</c> resource, as supplied via CDS Hooks prefetch.</summary>
public sealed class CoveragePayerExtractor
{
    private readonly FhirJsonSerializerService _serializerService;

    public CoveragePayerExtractor(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public string? ExtractPayerId(string? coverageResourceJson)
    {
        if (string.IsNullOrWhiteSpace(coverageResourceJson))
        {
            return null;
        }

        try
        {
            var coverage = _serializerService.Parse<Coverage>(coverageResourceJson);
            var payorReference = coverage.Payor.FirstOrDefault()?.Reference;
            return string.IsNullOrWhiteSpace(payorReference) ? null : payorReference.Split('/').Last();
        }
        catch
        {
            return null;
        }
    }
}
