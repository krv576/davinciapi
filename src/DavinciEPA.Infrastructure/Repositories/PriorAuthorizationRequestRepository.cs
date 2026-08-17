using DavinciEPA.Core.Entities;
using DavinciEPA.Core.Interfaces.Repositories;
using DavinciEPA.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DavinciEPA.Infrastructure.Repositories;

public sealed class PriorAuthorizationRequestRepository : IPriorAuthorizationRequestRepository
{
    private readonly DavinciEpaDbContext _dbContext;

    public PriorAuthorizationRequestRepository(DavinciEpaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<PriorAuthorizationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.PriorAuthorizationRequests
            .Include(r => r.CoverageRequirements)
            .Include(r => r.DocumentationRequirements)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<PriorAuthorizationRequest?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken) =>
        _dbContext.PriorAuthorizationRequests
            .Include(r => r.CoverageRequirements)
            .Include(r => r.DocumentationRequirements)
            .FirstOrDefaultAsync(r => r.ExternalId == externalId, cancellationToken);

    public async Task AddAsync(PriorAuthorizationRequest request, CancellationToken cancellationToken) =>
        await _dbContext.PriorAuthorizationRequests.AddAsync(request, cancellationToken);

    public void Update(PriorAuthorizationRequest request) => _dbContext.PriorAuthorizationRequests.Update(request);
}
