using DavinciEPA.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DavinciEPA.Infrastructure.Persistence.Configurations;

public sealed class RuleEvaluationLogConfiguration : IEntityTypeConfiguration<RuleEvaluationLog>
{
    public void Configure(EntityTypeBuilder<RuleEvaluationLog> builder)
    {
        builder.ToTable("RuleEvaluationLogs");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EngineType).HasConversion<string>().HasMaxLength(32);
        builder.Property(e => e.RuleId).IsRequired().HasMaxLength(64);
        builder.Property(e => e.InputSummary).IsRequired().HasMaxLength(1024);
        builder.Property(e => e.ResultSummary).IsRequired().HasMaxLength(1024);
        builder.Property(e => e.Severity).HasConversion<string>().HasMaxLength(32);

        builder.HasIndex(e => e.PriorAuthorizationRequestId);
    }
}

public sealed class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("AuditLogEntries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ActorId).IsRequired().HasMaxLength(128);
        builder.Property(e => e.Action).IsRequired().HasMaxLength(128);
        builder.Property(e => e.ResourceReference).IsRequired().HasMaxLength(256);

        builder.HasIndex(e => e.Timestamp);
    }
}
