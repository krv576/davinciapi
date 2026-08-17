using DavinciEPA.Core.Constants;
using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Fhir.Builders;

/// <summary>Assembles FHIR <c>Bundle</c> resources (collection and searchset) for the CRD/DTR/PAS interactions.</summary>
public sealed class BundleBuilder : IBundleBuilder
{
    private readonly FhirJsonSerializerService _serializerService;

    public BundleBuilder(FhirJsonSerializerService serializerService)
    {
        _serializerService = serializerService;
    }

    public string BuildCollectionBundleJson(IReadOnlyCollection<string> resourceJsonEntries)
    {
        var bundle = new Bundle
        {
            Id = Guid.NewGuid().ToString("N"),
            Meta = new Meta { Profile = new[] { DaVinciProfiles.PasBundle } },
            Type = Bundle.BundleType.Collection,
            Timestamp = DateTimeOffset.UtcNow
        };

        foreach (var json in resourceJsonEntries)
        {
            var resource = _serializerService.ParseResource(json);
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"urn:uuid:{Guid.NewGuid()}",
                Resource = resource
            });
        }

        return _serializerService.Serialize(bundle);
    }

    public string BuildSearchsetBundleJson(IReadOnlyCollection<string> resourceJsonEntries, int total)
    {
        var bundle = new Bundle
        {
            Id = Guid.NewGuid().ToString("N"),
            Type = Bundle.BundleType.Searchset,
            Total = total,
            Timestamp = DateTimeOffset.UtcNow
        };

        foreach (var json in resourceJsonEntries)
        {
            var resource = _serializerService.ParseResource(json);
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = string.IsNullOrWhiteSpace(resource.Id) ? null : $"{resource.TypeName}/{resource.Id}",
                Resource = resource,
                Search = new Bundle.SearchComponent { Mode = Bundle.SearchEntryMode.Match }
            });
        }

        return _serializerService.Serialize(bundle);
    }
}
