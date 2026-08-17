using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Infrastructure.Persistence;

namespace DavinciEPA.Infrastructure.Repositories;

/// <summary>Coordinates repository operations sharing a single <see cref="DavinciEpaDbContext"/> change tracker.</summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DavinciEpaDbContext _dbContext;

    public UnitOfWork(
        DavinciEpaDbContext dbContext,
        IPriorAuthorizationRequestRepository priorAuthorizationRequests,
        ICoverageRequirementRepository coverageRequirements,
        IDocumentationRequirementRepository documentationRequirements,
        IRuleEvaluationLogRepository ruleEvaluationLogs,
        IAuditLogRepository auditLogs)
    {
        _dbContext = dbContext;
        PriorAuthorizationRequests = priorAuthorizationRequests;
        CoverageRequirements = coverageRequirements;
        DocumentationRequirements = documentationRequirements;
        RuleEvaluationLogs = ruleEvaluationLogs;
        AuditLogs = auditLogs;
    }

    public IPriorAuthorizationRequestRepository PriorAuthorizationRequests { get; }

    public ICoverageRequirementRepository CoverageRequirements { get; }

    public IDocumentationRequirementRepository DocumentationRequirements { get; }

    public IRuleEvaluationLogRepository RuleEvaluationLogs { get; }

    public IAuditLogRepository AuditLogs { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
