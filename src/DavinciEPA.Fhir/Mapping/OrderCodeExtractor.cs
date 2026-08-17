using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Mapping;

/// <summary>Extracts the ordered procedure/product/service code(s) from a CRD order resource (ServiceRequest, DeviceRequest, or MedicationRequest).</summary>
public sealed class OrderCodeExtractor
{
    private readonly FhirJsonSerializerService _serializerService;

    public OrderCodeExtractor(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public IReadOnlyCollection<string> ExtractProcedureCodes(string orderResourceJson)
    {
        Resource resource;
        try
        {
            resource = _serializerService.ParseResource(orderResourceJson);
        }
        catch
        {
            return Array.Empty<string>();
        }

        var codings = resource switch
        {
            ServiceRequest serviceRequest => serviceRequest.Code?.Coding,
            DeviceRequest deviceRequest => (deviceRequest.Code as CodeableConcept)?.Coding,
            MedicationRequest medicationRequest => (medicationRequest.Medication as CodeableConcept)?.Coding,
            _ => null
        };

        if (codings is null)
        {
            return Array.Empty<string>();
        }

        return codings
            .Select(coding => coding.Code)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code!)
            .ToList();
    }
}
