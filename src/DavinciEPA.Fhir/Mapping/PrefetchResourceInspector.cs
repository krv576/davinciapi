using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Mapping;

/// <summary>Inspects CDS Hooks prefetch (or PAS supporting-info) FHIR resources for coded clinical data relevant to rule evaluation.</summary>
public sealed class PrefetchResourceInspector
{
    private readonly FhirJsonSerializerService _serializerService;

    public PrefetchResourceInspector(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public IReadOnlyCollection<string> ExtractConditionCodes(IReadOnlyDictionary<string, string> resourcesJson)
    {
        var codes = new List<string>();

        foreach (var json in resourcesJson.Values)
        {
            foreach (var resource in ParseAsResources(json))
            {
                if (resource is Condition condition)
                {
                    codes.AddRange(condition.Code?.Coding
                        .Select(coding => coding.Code)
                        .Where(code => !string.IsNullOrWhiteSpace(code))
                        .Select(code => code!) ?? Enumerable.Empty<string>());
                }
            }
        }

        return codes;
    }

    private IEnumerable<Resource> ParseAsResources(string json)
    {
        Resource resource;
        try
        {
            resource = _serializerService.ParseResource(json);
        }
        catch
        {
            yield break;
        }

        if (resource is Bundle bundle)
        {
            foreach (var entry in bundle.Entry)
            {
                if (entry.Resource is not null)
                {
                    yield return entry.Resource;
                }
            }
        }
        else
        {
            yield return resource;
        }
    }
}
