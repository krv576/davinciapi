using DavinciEPA.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DavinciEPA.Infrastructure.Persistence.Configurations;

public sealed class CoverageRequirementEvaluationConfiguration : IEntityTypeConfiguration<CoverageRequirementEvaluation>
{
    public void Configure(EntityTypeBuilder<CoverageRequirementEvaluation> builder)
    {
        builder.ToTable("CoverageRequirementEvaluations");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrderReference).IsRequired().HasMaxLength(256);
        builder.Property(e => e.RequirementCode).IsRequired().HasMaxLength(64);
        builder.Property(e => e.RequirementDescription).IsRequired().HasMaxLength(512);

        builder.HasIndex(e => e.OrderReference);
        builder.HasIndex(e => e.PriorAuthorizationRequestId);
    }
}

public sealed class DocumentationRequirementConfiguration : IEntityTypeConfiguration<DocumentationRequirement>
{
    public void Configure(EntityTypeBuilder<DocumentationRequirement> builder)
    {
        builder.ToTable("DocumentationRequirements");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.QuestionnaireCanonicalUrl).IsRequired().HasMaxLength(512);
        builder.Property(e => e.QuestionnaireResponseReference).HasMaxLength(256);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(32);

        builder.HasIndex(e => e.PriorAuthorizationRequestId);
    }
}
