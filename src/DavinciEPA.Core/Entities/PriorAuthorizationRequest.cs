using DavinciEPA.Core.Enums;

namespace DavinciEPA.Core.Entities;

/// <summary>
/// Aggregate root tracking a single prior authorization request across the CRD, DTR, and PAS workflow.
/// </summary>
public sealed class PriorAuthorizationRequest
{
    private readonly List<CoverageRequirementEvaluation> _coverageRequirements = new();
    private readonly List<DocumentationRequirement> _documentationRequirements = new();

    private PriorAuthorizationRequest()
    {
        // Reserved for EF Core materialization.
        ExternalId = string.Empty;
        PatientIdentifier = string.Empty;
        PayerId = string.Empty;
        OrderReference = string.Empty;
    }

    public PriorAuthorizationRequest(
        Guid id,
        string externalId,
        string patientIdentifier,
        string payerId,
        string orderReference,
        DateTimeOffset createdAt)
    {
        Id = id;
        ExternalId = externalId;
        PatientIdentifier = patientIdentifier;
        PayerId = payerId;
        OrderReference = orderReference;
        Status = PriorAuthorizationStatus.Draft;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    /// <summary>Business identifier correlating to the FHIR <c>Claim</c> submitted to PAS.</summary>
    public string ExternalId { get; private set; }

    public string PatientIdentifier { get; private set; }

    public string PayerId { get; private set; }

    /// <summary>FHIR reference to the triggering order (ServiceRequest/DeviceRequest/MedicationRequest).</summary>
    public string OrderReference { get; private set; }

    public PriorAuthorizationStatus Status { get; private set; }

    public PriorAuthorizationDisposition? Disposition { get; private set; }

    public string? DispositionReason { get; private set; }

    /// <summary>Task identifier used for asynchronous ($inquire) status polling once pended.</summary>
    public string? TaskIdentifier { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<CoverageRequirementEvaluation> CoverageRequirements => _coverageRequirements;

    public IReadOnlyCollection<DocumentationRequirement> DocumentationRequirements => _documentationRequirements;

    public void AddCoverageRequirement(CoverageRequirementEvaluation evaluation)
    {
        _coverageRequirements.Add(evaluation);
        if (!evaluation.IsMet)
        {
            Status = PriorAuthorizationStatus.DocumentationRequired;
        }

        Touch();
    }

    public void AddDocumentationRequirement(DocumentationRequirement requirement)
    {
        _documentationRequirements.Add(requirement);
        Status = PriorAuthorizationStatus.DocumentationInProgress;
        Touch();
    }

    public void MarkReadyToSubmit()
    {
        if (_documentationRequirements.Any(d => d.Status != DocumentationRequirementStatus.Completed))
        {
            throw new InvalidOperationException(
                "All documentation requirements must be completed before the request can be submitted.");
        }

        Status = PriorAuthorizationStatus.ReadyToSubmit;
        Touch();
    }

    public void MarkSubmitted()
    {
        Status = PriorAuthorizationStatus.Submitted;
        Touch();
    }

    public void MarkPended(string taskIdentifier)
    {
        TaskIdentifier = taskIdentifier;
        Status = PriorAuthorizationStatus.Pended;
        Disposition = PriorAuthorizationDisposition.Pending;
        Touch();
    }

    public void RecordDecision(PriorAuthorizationDisposition disposition, string? reason)
    {
        Disposition = disposition;
        DispositionReason = reason;
        Status = disposition switch
        {
            PriorAuthorizationDisposition.Granted => PriorAuthorizationStatus.Approved,
            PriorAuthorizationDisposition.PartiallyGranted => PriorAuthorizationStatus.Approved,
            PriorAuthorizationDisposition.Denied => PriorAuthorizationStatus.Denied,
            PriorAuthorizationDisposition.Cancelled => PriorAuthorizationStatus.Cancelled,
            _ => PriorAuthorizationStatus.Pended
        };
        Touch();
    }

    public void MarkError()
    {
        Status = PriorAuthorizationStatus.Error;
        Touch();
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
