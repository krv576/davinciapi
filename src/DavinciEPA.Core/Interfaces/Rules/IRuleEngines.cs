using DavinciEPA.Core.DTOs;

namespace DavinciEPA.Core.Interfaces.Rules;

/// <summary>Evaluates payer coverage requirement rules for an order in context (CRD).</summary>
public interface ICoverageRuleEngine
{
    Task<IReadOnlyCollection<CoverageRequirementResultDto>> EvaluateAsync(
        CoverageRequirementDiscoveryRequestDto request,
        CancellationToken cancellationToken);
}

/// <summary>Outcome of evaluating clinical (medical necessity) criteria against a patient's data.</summary>
public sealed record MedicalNecessityEvaluationResult(bool IsMet, IReadOnlyCollection<string> UnmetCriteria);

/// <summary>Evaluates DTR questionnaire pre-population rules against a patient's EHR data (auto-population).</summary>
public interface IQuestionnairePrePopulationEngine
{
    Task<IReadOnlyDictionary<string, string>> PrepopulateAsync(
        string questionnaireCanonicalUrl,
        string patientFhirId,
        string fhirServerBaseUrl,
        string? accessToken,
        CancellationToken cancellationToken);
}

/// <summary>Evaluates medical necessity clinical criteria for a requested service/order.</summary>
public interface IMedicalNecessityRuleEngine
{
    Task<MedicalNecessityEvaluationResult> EvaluateAsync(
        string orderResourceJson,
        IReadOnlyDictionary<string, string> prefetchResourcesJson,
        CancellationToken cancellationToken);
}

/// <summary>Final decision produced by combining coverage, documentation, and medical necessity evaluations.</summary>
public sealed record PriorAuthorizationRuleDecision(
    Enums.PriorAuthorizationDisposition Disposition,
    string Reason);

/// <summary>Evaluates the overall prior authorization decision once all supporting evidence is available (PAS).</summary>
public interface IPriorAuthorizationRuleEngine
{
    Task<PriorAuthorizationRuleDecision> EvaluateAsync(
        string claimResourceJson,
        IReadOnlyCollection<string> supportingResourceJsonEntries,
        CancellationToken cancellationToken);
}
