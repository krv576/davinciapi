namespace DavinciEPA.Core.Entities;

/// <summary>System-wide, PHI-free audit trail entry recording who did what to which resource and when.</summary>
public sealed class AuditLogEntry
{
    private AuditLogEntry()
    {
        ActorId = string.Empty;
        Action = string.Empty;
        ResourceReference = string.Empty;
    }

    public AuditLogEntry(Guid id, string actorId, string action, string resourceReference, DateTimeOffset timestamp)
    {
        Id = id;
        ActorId = actorId;
        Action = action;
        ResourceReference = resourceReference;
        Timestamp = timestamp;
    }

    public Guid Id { get; private set; }

    /// <summary>Client/user identifier extracted from the validated access token.</summary>
    public string ActorId { get; private set; }

    /// <summary>e.g. "Claim.Submit", "QuestionnaireResponse.Submit", "CoverageRequirement.Evaluate".</summary>
    public string Action { get; private set; }

    public string ResourceReference { get; private set; }

    public DateTimeOffset Timestamp { get; private set; }
}
