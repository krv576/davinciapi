using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Mapping;

/// <summary>Extracts the resource reference from a submitted DTR <c>QuestionnaireResponse</c>.</summary>
public sealed class QuestionnaireResponseExtractor
{
    private readonly FhirJsonSerializerService _serializerService;

    public QuestionnaireResponseExtractor(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public string ExtractReference(string questionnaireResponseJson)
    {
        var response = _serializerService.Parse<QuestionnaireResponse>(questionnaireResponseJson);
        var id = string.IsNullOrWhiteSpace(response.Id) ? Guid.NewGuid().ToString("N") : response.Id;
        return $"QuestionnaireResponse/{id}";
    }
}
