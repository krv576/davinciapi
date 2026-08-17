using DavinciEPA.Core.DTOs;
using DavinciEPA.Core.Entities;
using DavinciEPA.Core.Enums;
using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Core.Interfaces.Services;
using DavinciEPA.Core.Results;

namespace DavinciEPA.Core.Services;

/// <summary>
/// Implements Coverage Requirements Discovery for a single CDS Hooks invocation: evaluates coverage rules
/// for the order in context and persists both the requirement outcomes and an audit trail of the evaluation.
/// </summary>
public sealed class CoverageRequirementDiscoveryService : ICoverageRequirementDiscoveryService
{
    private readonly ICoverageRuleEngine _ruleEngine;
    private readonly IUnitOfWork _unitOfWork;

    public CoverageRequirementDiscoveryService(ICoverageRuleEngine ruleEngine, IUnitOfWork unitOfWork)
    {
        _ruleEngine = ruleEngine;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CoverageRequirementDiscoveryResultDto>> DiscoverAsync(
        CoverageRequirementDiscoveryRequestDto request,
        CancellationToken cancellationToken)
    {
        var results = await _ruleEngine.EvaluateAsync(request, cancellationToken);
        var evaluatedAt = DateTimeOffset.UtcNow;

        foreach (var result in results)
        {
            var evaluation = new CoverageRequirementEvaluation(
                Guid.NewGuid(),
                priorAuthorizationRequestId: null,
                request.OrderReference,
                result.RequirementCode,
                result.RequirementDescription,
                result.IsMet,
                evaluatedAt);

            await _unitOfWork.CoverageRequirements.AddAsync(evaluation, cancellationToken);

            var log = new RuleEvaluationLog(
                Guid.NewGuid(),
                priorAuthorizationRequestId: null,
                RuleEngineType.Coverage,
                result.RequirementCode,
                inputSummary: $"order={request.OrderReference};payer={request.PayerId}",
                resultSummary: $"isMet={result.IsMet}",
                severity: result.IsMet ? RuleSeverity.Information : RuleSeverity.Warning,
                evaluatedAt);

            await _unitOfWork.RuleEvaluationLogs.AddAsync(log, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CoverageRequirementDiscoveryResultDto(request.OrderReference, results));
    }
}
