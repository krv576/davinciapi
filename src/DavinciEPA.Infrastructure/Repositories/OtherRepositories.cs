using DavinciEPA.Core.Entities;
using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DavinciEPA.Infrastructure.Repositories;

public sealed class CoverageRequirementRepository : ICoverageRequirementRepository
{
    private readonly DavinciEpaDbContext _dbContext;

    public CoverageRequirementRepository(DavinciEpaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(CoverageRequirementEvaluation evaluation, CancellationToken cancellationToken) =>
        await _dbContext.CoverageRequirementEvaluations.AddAsync(evaluation, cancellationToken);

    public async Task<IReadOnlyCollection<CoverageRequirementEvaluation>> GetByOrderReferenceAsync(
        string orderReference,
        CancellationToken cancellationToken) =>
        await _dbContext.CoverageRequirementEvaluations
            .Where(e => e.OrderReference == orderReference)
            .ToListAsync(cancellationToken);
}

public sealed class DocumentationRequirementRepository : IDocumentationRequirementRepository
{
    private readonly DavinciEpaDbContext _dbContext;

    public DocumentationRequirementRepository(DavinciEpaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<DocumentationRequirement?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.DocumentationRequirements.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task AddAsync(DocumentationRequirement requirement, CancellationToken cancellationToken) =>
        await _dbContext.DocumentationRequirements.AddAsync(requirement, cancellationToken);

    public void Update(DocumentationRequirement requirement) => _dbContext.DocumentationRequirements.Update(requirement);
}

public sealed class RuleEvaluationLogRepository : IRuleEvaluationLogRepository
{
    private readonly DavinciEpaDbContext _dbContext;

    public RuleEvaluationLogRepository(DavinciEpaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RuleEvaluationLog log, CancellationToken cancellationToken) =>
        await _dbContext.RuleEvaluationLogs.AddAsync(log, cancellationToken);
}

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly DavinciEpaDbContext _dbContext;

    public AuditLogRepository(DavinciEpaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken) =>
        await _dbContext.AuditLogEntries.AddAsync(entry, cancellationToken);
}
