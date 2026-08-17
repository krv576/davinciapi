using DavinciEPA.Core.Enums;

namespace DavinciEPA.Core.Entities;

/// <summary>Auditable record of a single rule engine evaluation, independent of the resulting requirement entities.</summary>
public sealed class RuleEvaluationLog
{
    private RuleEvaluationLog()
    {
        RuleId = string.Empty;
        InputSummary = string.Empty;
        ResultSummary = string.Empty;
    }

    public RuleEvaluationLog(
        Guid id,
        Guid? priorAuthorizationRequestId,
        RuleEngineType engineType,
        string ruleId,
        string inputSummary,
        string resultSummary,
        RuleSeverity severity,
        DateTimeOffset evaluatedAt)
    {
        Id = id;
        PriorAuthorizationRequestId = priorAuthorizationRequestId;
        EngineType = engineType;
        RuleId = ruleId;
        InputSummary = inputSummary;
        ResultSummary = resultSummary;
        Severity = severity;
        EvaluatedAt = evaluatedAt;
    }

    public Guid Id { get; private set; }

    public Guid? PriorAuthorizationRequestId { get; private set; }

    public RuleEngineType EngineType { get; private set; }

    public string RuleId { get; private set; }

    /// <summary>Non-PHI summary of the coded inputs evaluated (e.g. procedure/diagnosis codes), never free-text clinical notes.</summary>
    public string InputSummary { get; private set; }

    public string ResultSummary { get; private set; }

    public RuleSeverity Severity { get; private set; }

    public DateTimeOffset EvaluatedAt { get; private set; }
}
