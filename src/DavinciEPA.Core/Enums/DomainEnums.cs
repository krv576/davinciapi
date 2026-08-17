namespace DavinciEPA.Core.Enums;

/// <summary>Lifecycle status of a prior authorization request as it moves through CRD, DTR, and PAS.</summary>
public enum PriorAuthorizationStatus
{
    Draft,
    DocumentationRequired,
    DocumentationInProgress,
    ReadyToSubmit,
    Submitted,
    Pended,
    AdditionalInformationRequired,
    Approved,
    Denied,
    Cancelled,
    Error
}

/// <summary>Final payer disposition of a prior authorization decision, modeled after the PAS IG's adjudication outcomes.</summary>
public enum PriorAuthorizationDisposition
{
    Pending,
    Granted,
    Denied,
    PartiallyGranted,
    Cancelled
}

/// <summary>Completion status of a DTR documentation requirement (Questionnaire/QuestionnaireResponse cycle).</summary>
public enum DocumentationRequirementStatus
{
    NotStarted,
    InProgress,
    Completed
}

/// <summary>Severity of a single rule evaluation finding.</summary>
public enum RuleSeverity
{
    Information,
    Warning,
    Error
}

/// <summary>Which rule engine produced a given evaluation, for audit/traceability.</summary>
public enum RuleEngineType
{
    Coverage,
    PriorAuthorization,
    MedicalNecessity
}
