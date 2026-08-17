using DavinciEPA.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DavinciEPA.Infrastructure.Persistence;

/// <summary>EF Core persistence context for the entire Da Vinci EPA platform.</summary>
public sealed class DavinciEpaDbContext : DbContext
{
    public DavinciEpaDbContext(DbContextOptions<DavinciEpaDbContext> options) : base(options)
    {
    }

    public DbSet<PriorAuthorizationRequest> PriorAuthorizationRequests => Set<PriorAuthorizationRequest>();

    public DbSet<CoverageRequirementEvaluation> CoverageRequirementEvaluations => Set<CoverageRequirementEvaluation>();

    public DbSet<DocumentationRequirement> DocumentationRequirements => Set<DocumentationRequirement>();

    public DbSet<RuleEvaluationLog> RuleEvaluationLogs => Set<RuleEvaluationLog>();

    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DavinciEpaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
