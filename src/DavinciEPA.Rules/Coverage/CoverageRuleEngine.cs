using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Fhir.Mapping;

namespace DavinciEPA.Rules.Coverage;

/// <summary>
/// Evaluates the <see cref="CoverageRuleCatalog"/> against the procedure/product codes on the order in
/// context for a CDS Hooks invocation (Coverage Requirements Discovery).
/// </summary>
public sealed class CoverageRuleEngine : ICoverageRuleEngine
{
    private readonly OrderCodeExtractor _orderCodeExtractor;

    public CoverageRuleEngine(OrderCodeExtractor orderCodeExtractor)
    {
        _orderCodeExtractor = orderCodeExtractor;
    }

    public Task<IReadOnlyCollection<CoverageRequirementResultDto>> EvaluateAsync(
        CoverageRequirementDiscoveryRequestDto request,
        CancellationToken cancellationToken)
    {
        var orderCodes = _orderCodeExtractor.ExtractProcedureCodes(request.OrderResourceJson);

        var results = CoverageRuleCatalog.Rules
            .Where(rule => rule.ApplicableProcedureCodes.Any(code => orderCodes.Contains(code, StringComparer.OrdinalIgnoreCase)))
            .Select(rule => new CoverageRequirementResultDto(
                rule.RequirementCode,
                rule.Description,
                IsMet: false,
                rule.DocumentationQuestionnaireCanonicalUrl))
            .ToList();

        if (results.Count == 0)
        {
            results.Add(new CoverageRequirementResultDto(
                "PA-NOT-REQUIRED",
                "No prior authorization requirement was identified for the code(s) on this order.",
                IsMet: true,
                DocumentationQuestionnaireCanonicalUrl: null));
        }

        return Task.FromResult<IReadOnlyCollection<CoverageRequirementResultDto>>(results);
    }
}
