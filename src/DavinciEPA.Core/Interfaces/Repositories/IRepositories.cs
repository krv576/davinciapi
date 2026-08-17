using DavinciEPA.Core.Entities;

namespace DavinciEPA.Core.Interfaces.Repositories;

/// <summary>Persistence port for the <see cref="PriorAuthorizationRequest"/> aggregate.</summary>
public interface IPriorAuthorizationRequestRepository
{
    Task<PriorAuthorizationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PriorAuthorizationRequest?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken);

    Task AddAsync(PriorAuthorizationRequest request, CancellationToken cancellationToken);

    void Update(PriorAuthorizationRequest request);
}

/// <summary>Persistence port for standalone coverage requirement evaluations (e.g. CRD-only lookups).</summary>
public interface ICoverageRequirementRepository
{
    Task AddAsync(CoverageRequirementEvaluation evaluation, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CoverageRequirementEvaluation>> GetByOrderReferenceAsync(
        string orderReference,
        CancellationToken cancellationToken);
}

/// <summary>Persistence port for DTR documentation requirements.</summary>
public interface IDocumentationRequirementRepository
{
    Task<DocumentationRequirement?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(DocumentationRequirement requirement, CancellationToken cancellationToken);

    void Update(DocumentationRequirement requirement);
}

/// <summary>Persistence port for rule engine audit records.</summary>
public interface IRuleEvaluationLogRepository
{
    Task AddAsync(RuleEvaluationLog log, CancellationToken cancellationToken);
}

/// <summary>Persistence port for system-wide audit trail entries.</summary>
public interface IAuditLogRepository
{
    Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken);
}

/// <summary>Coordinates a single unit of work across repositories backed by the same persistence context.</summary>
public interface IUnitOfWork
{
    IPriorAuthorizationRequestRepository PriorAuthorizationRequests { get; }

    ICoverageRequirementRepository CoverageRequirements { get; }

    IDocumentationRequirementRepository DocumentationRequirements { get; }

    IRuleEvaluationLogRepository RuleEvaluationLogs { get; }

    IAuditLogRepository AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
