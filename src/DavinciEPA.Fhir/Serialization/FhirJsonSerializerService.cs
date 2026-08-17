using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace DavinciEPA.Fhir.Serialization;

/// <summary>
/// Thin wrapper around the Firely SDK's JSON (de)serializer. Internal to <c>DavinciEPA.Fhir</c> — no other
/// project should serialize/parse FHIR resources directly.
/// </summary>
public sealed class FhirJsonSerializerService
{
    private readonly FhirJsonParser _parser;
    private readonly FhirJsonSerializer _serializer;

    public FhirJsonSerializerService()
    {
        _parser = new FhirJsonParser(new ParserSettings
        {
            AcceptUnknownMembers = true,
            AllowUnrecognizedEnums = true
        });

        _serializer = new FhirJsonSerializer(new SerializerSettings
        {
            Pretty = false
        });
    }

    public string Serialize(Resource resource) => _serializer.SerializeToString(resource);

    public T Parse<T>(string json) where T : Resource => _parser.Parse<T>(json);

    public Resource ParseResource(string json) => _parser.Parse<Resource>(json);
}
