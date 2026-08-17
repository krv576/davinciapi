using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Fhir.Mapping;

namespace DavinciEPA.Rules.MedicalNecessity;

/// <summary>Evaluates whether the patient's coded clinical data satisfies the medical necessity criteria for any applicable rule.</summary>
public sealed class MedicalNecessityRuleEngine : IMedicalNecessityRuleEngine
{
    private readonly PrefetchResourceInspector _inspector;

    public MedicalNecessityRuleEngine(PrefetchResourceInspector inspector)
    {
        _inspector = inspector;
    }

    public Task<MedicalNecessityEvaluationResult> EvaluateAsync(
        string orderResourceJson,
        IReadOnlyDictionary<string, string> prefetchResourcesJson,
        CancellationToken cancellationToken)
    {
        var conditionCodes = _inspector.ExtractConditionCodes(prefetchResourcesJson);

        var hasQualifyingDiagnosis = MedicalNecessityCriteriaCatalog.Criteria
            .SelectMany(criterion => criterion.QualifyingConditionCodes)
            .Any(code => conditionCodes.Contains(code, StringComparer.OrdinalIgnoreCase));

        var unmet = hasQualifyingDiagnosis
            ? Array.Empty<string>()
            : new[] { "No qualifying diagnosis code was found among the patient's active conditions." };

        return Task.FromResult(new MedicalNecessityEvaluationResult(hasQualifyingDiagnosis, unmet));
    }
}
