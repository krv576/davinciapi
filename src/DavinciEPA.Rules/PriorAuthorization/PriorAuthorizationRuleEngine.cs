using DavinciEPA.Core.Enums;
using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Fhir.Mapping;
using DavinciEPA.Rules.Coverage;
using DavinciEPA.Rules.MedicalNecessity;

namespace DavinciEPA.Rules.PriorAuthorization;

/// <summary>
/// Produces the final PAS adjudication decision by combining the submitted Claim's requested service against
/// the coverage rule catalog and, when supporting clinical evidence is present, the medical necessity criteria.
/// </summary>
public sealed class PriorAuthorizationRuleEngine : IPriorAuthorizationRuleEngine
{
    private readonly PasBundleExtractor _bundleExtractor;
    private readonly PrefetchResourceInspector _inspector;

    public PriorAuthorizationRuleEngine(PasBundleExtractor bundleExtractor, PrefetchResourceInspector inspector)
    {
        _bundleExtractor = bundleExtractor;
        _inspector = inspector;
    }

    public Task<PriorAuthorizationRuleDecision> EvaluateAsync(
        string claimResourceJson,
        IReadOnlyCollection<string> supportingResourceJsonEntries,
        CancellationToken cancellationToken)
    {
        PasSubmissionContext context;
        try
        {
            context = _bundleExtractor.Extract(claimResourceJson);
        }
        catch (Exception ex)
        {
            return Task.FromResult(new PriorAuthorizationRuleDecision(
                PriorAuthorizationDisposition.Denied,
                $"Submitted bundle did not contain a valid Claim resource: {ex.Message}"));
        }

        var matchingRule = CoverageRuleCatalog.Rules.FirstOrDefault(
            rule => rule.ApplicableProcedureCodes.Contains(context.OrderReference, StringComparer.OrdinalIgnoreCase));

        if (matchingRule is null)
        {
            return Task.FromResult(new PriorAuthorizationRuleDecision(
                PriorAuthorizationDisposition.Granted,
                "No prior authorization rule applies to the requested service; auto-approved."));
        }

        if (supportingResourceJsonEntries.Count == 0)
        {
            return Task.FromResult(new PriorAuthorizationRuleDecision(
                PriorAuthorizationDisposition.Pending,
                $"Awaiting supporting clinical documentation for requirement '{matchingRule.RequirementCode}'."));
        }

        var supportingResourcesByKey = supportingResourceJsonEntries
            .Select((json, index) => (Key: $"supporting-{index}", Json: json))
            .ToDictionary(entry => entry.Key, entry => entry.Json);

        var conditionCodes = _inspector.ExtractConditionCodes(supportingResourcesByKey);

        var hasQualifyingDiagnosis = MedicalNecessityCriteriaCatalog.Criteria
            .Where(criterion => criterion.RequirementCode == matchingRule.RequirementCode)
            .SelectMany(criterion => criterion.QualifyingConditionCodes)
            .Any(code => conditionCodes.Contains(code, StringComparer.OrdinalIgnoreCase));

        var decision = hasQualifyingDiagnosis
            ? new PriorAuthorizationRuleDecision(
                PriorAuthorizationDisposition.Granted,
                $"Medical necessity criteria met for requirement '{matchingRule.RequirementCode}'.")
            : new PriorAuthorizationRuleDecision(
                PriorAuthorizationDisposition.Denied,
                $"Medical necessity criteria not met for requirement '{matchingRule.RequirementCode}'.");

        return Task.FromResult(decision);
    }
}
