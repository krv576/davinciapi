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

    private const string FallbackQuestionnaireCanonicalUrl = "http://localhost:5027/fhir/Questionnaire/advanced-imaging";

    public async Task<Result<CoverageRequirementDiscoveryResultDto>> DiscoverAsync(
        CoverageRequirementDiscoveryRequestDto request,
        CancellationToken cancellationToken)
    {
        var results = await _ruleEngine.EvaluateAsync(request, cancellationToken);
        var evaluatedAt = DateTimeOffset.UtcNow;

        // Lazily created once per request, and reused across every unmet requirement so all
        // resulting records (evaluation, log, documentation requirement) share one aggregate.
        PriorAuthorizationRequest? priorAuthorizationRequest = null;

        foreach (var result in results)
        {
            Guid? priorAuthorizationRequestId = null;

            if (!result.IsMet)
            {
                if (priorAuthorizationRequest is null)
                {
                    priorAuthorizationRequest = new PriorAuthorizationRequest(
                        Guid.NewGuid(),
                        request.OrderReference,
                        request.PatientIdentifier,
                        request.PayerId,
                        request.OrderReference,
                        evaluatedAt);

                    await _unitOfWork.PriorAuthorizationRequests.AddAsync(priorAuthorizationRequest, cancellationToken);
                }

                priorAuthorizationRequestId = priorAuthorizationRequest.Id;
            }

            var evaluation = new CoverageRequirementEvaluation(
                Guid.NewGuid(),
                priorAuthorizationRequestId,
                request.OrderReference,
                result.RequirementCode,
                result.RequirementDescription,
                result.IsMet,
                evaluatedAt);

            await _unitOfWork.CoverageRequirements.AddAsync(evaluation, cancellationToken);

            if (priorAuthorizationRequest is not null && priorAuthorizationRequestId is not null)
            {
                priorAuthorizationRequest.AddCoverageRequirement(evaluation);
            }

            var log = new RuleEvaluationLog(
                Guid.NewGuid(),
                priorAuthorizationRequestId,
                RuleEngineType.Coverage,
                result.RequirementCode,
                inputSummary: $"order={request.OrderReference};payer={request.PayerId}",
                resultSummary: $"isMet={result.IsMet}",
                severity: result.IsMet ? RuleSeverity.Information : RuleSeverity.Warning,
                evaluatedAt);

            await _unitOfWork.RuleEvaluationLogs.AddAsync(log, cancellationToken);

            if (!result.IsMet && priorAuthorizationRequest is not null)
            {
                var questionnaireCanonicalUrl = result.DocumentationQuestionnaireCanonicalUrl ?? FallbackQuestionnaireCanonicalUrl;

                // Reuse an existing requirement for the same questionnaire instead of duplicating it.
                var documentationRequirement = priorAuthorizationRequest.DocumentationRequirements
                    .FirstOrDefault(d => d.QuestionnaireCanonicalUrl == questionnaireCanonicalUrl);

                if (documentationRequirement is null)
                {
                    documentationRequirement = new DocumentationRequirement(
                        Guid.NewGuid(),
                        priorAuthorizationRequest.Id,
                        questionnaireCanonicalUrl,
                        evaluatedAt);

                    priorAuthorizationRequest.AddDocumentationRequirement(documentationRequirement);
                    await _unitOfWork.DocumentationRequirements.AddAsync(documentationRequirement, cancellationToken);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CoverageRequirementDiscoveryResultDto(request.OrderReference, results));
    }
}
