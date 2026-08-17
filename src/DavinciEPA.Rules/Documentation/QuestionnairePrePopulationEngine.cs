using DavinciEPA.Core.Interfaces.External;
using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Fhir.Serialization;
using Hl7.Fhir.Model;

namespace DavinciEPA.Rules.Documentation;

/// <summary>
/// Evaluates DTR questionnaire pre-population rules by querying the launching EHR for the patient's active
/// conditions and using them to auto-populate the corresponding Questionnaire answers.
/// </summary>
public sealed class QuestionnairePrePopulationEngine : IQuestionnairePrePopulationEngine
{
    private readonly IEhrFhirClient _ehrFhirClient;
    private readonly FhirJsonSerializerService _serializerService;

    public QuestionnairePrePopulationEngine(IEhrFhirClient ehrFhirClient, FhirJsonSerializerService serializerService)
    {
        _ehrFhirClient = ehrFhirClient;
        _serializerService = serializerService;
    }

    public async Task<IReadOnlyDictionary<string, string>> PrepopulateAsync(
        string questionnaireCanonicalUrl,
        string patientFhirId,
        string fhirServerBaseUrl,
        string? accessToken,
        CancellationToken cancellationToken)
    {
        var answers = new Dictionary<string, string>();

        var conditionsJson = await _ehrFhirClient.SearchAsync(
            fhirServerBaseUrl,
            accessToken,
            "Condition",
            new Dictionary<string, string> { ["patient"] = patientFhirId, ["clinical-status"] = "active" },
            cancellationToken);

        if (string.IsNullOrWhiteSpace(conditionsJson))
        {
            return answers;
        }

        try
        {
            var bundle = _serializerService.Parse<Bundle>(conditionsJson);
            var condition = bundle.Entry.Select(entry => entry.Resource).OfType<Condition>().FirstOrDefault();

            var code = condition?.Code?.Coding.FirstOrDefault()?.Code;
            if (!string.IsNullOrWhiteSpace(code))
            {
                answers["diagnosis-code"] = code;
            }

            var text = condition?.Code?.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                answers["clinical-indication"] = text;
            }
        }
        catch
        {
            // Prefetch data that fails to parse is skipped; the questionnaire is still returned unpopulated.
        }

        return answers;
    }
}
