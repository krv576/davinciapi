using DavinciEPA.Core.Enums;

namespace DavinciEPA.Core.Entities;

/// <summary>Result of evaluating a single CRD coverage requirement rule against an order in context.</summary>
public sealed class CoverageRequirementEvaluation
{
    private CoverageRequirementEvaluation()
    {
        OrderReference = string.Empty;
        RequirementCode = string.Empty;
        RequirementDescription = string.Empty;
    }

    public CoverageRequirementEvaluation(
        Guid id,
        Guid? priorAuthorizationRequestId,
        string orderReference,
        string requirementCode,
        string requirementDescription,
        bool isMet,
        DateTimeOffset evaluatedAt)
    {
        Id = id;
        PriorAuthorizationRequestId = priorAuthorizationRequestId;
        OrderReference = orderReference;
        RequirementCode = requirementCode;
        RequirementDescription = requirementDescription;
        IsMet = isMet;
        EvaluatedAt = evaluatedAt;
    }

    public Guid Id { get; private set; }

    public Guid? PriorAuthorizationRequestId { get; private set; }

    public string OrderReference { get; private set; }

    public string RequirementCode { get; private set; }

    public string RequirementDescription { get; private set; }

    public bool IsMet { get; private set; }

    public DateTimeOffset EvaluatedAt { get; private set; }
}

/// <summary>Tracks DTR questionnaire progress for a single documentation requirement raised during CRD.</summary>
public sealed class DocumentationRequirement
{
    private DocumentationRequirement()
    {
        QuestionnaireCanonicalUrl = string.Empty;
    }

    public DocumentationRequirement(
        Guid id,
        Guid priorAuthorizationRequestId,
        string questionnaireCanonicalUrl,
        DateTimeOffset createdAt)
    {
        Id = id;
        PriorAuthorizationRequestId = priorAuthorizationRequestId;
        QuestionnaireCanonicalUrl = questionnaireCanonicalUrl;
        Status = DocumentationRequirementStatus.NotStarted;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid PriorAuthorizationRequestId { get; private set; }

    public string QuestionnaireCanonicalUrl { get; private set; }

    public string? QuestionnaireResponseReference { get; private set; }

    public DocumentationRequirementStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public void MarkInProgress()
    {
        Status = DocumentationRequirementStatus.InProgress;
    }

    public void Complete(string questionnaireResponseReference)
    {
        QuestionnaireResponseReference = questionnaireResponseReference;
        Status = DocumentationRequirementStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
    }
}
