using DavinciEPA.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DavinciEPA.Infrastructure.Persistence.Configurations;

public sealed class PriorAuthorizationRequestConfiguration : IEntityTypeConfiguration<PriorAuthorizationRequest>
{
    public void Configure(EntityTypeBuilder<PriorAuthorizationRequest> builder)
    {
        builder.ToTable("PriorAuthorizationRequests");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExternalId).IsRequired().HasMaxLength(128);
        builder.HasIndex(e => e.ExternalId).IsUnique();

        builder.Property(e => e.PatientIdentifier).IsRequired().HasMaxLength(128);
        builder.HasIndex(e => e.PatientIdentifier);

        builder.Property(e => e.PayerId).IsRequired().HasMaxLength(128);
        builder.Property(e => e.OrderReference).IsRequired().HasMaxLength(256);

        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(64);
        builder.HasIndex(e => e.Status);

        builder.Property(e => e.Disposition).HasConversion<string>().HasMaxLength(64);
        builder.Property(e => e.DispositionReason).HasMaxLength(1024);
        builder.Property(e => e.TaskIdentifier).HasMaxLength(128);

        builder.Metadata.FindNavigation(nameof(PriorAuthorizationRequest.CoverageRequirements))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(PriorAuthorizationRequest.DocumentationRequirements))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.CoverageRequirements)
            .WithOne()
            .HasForeignKey(e => e.PriorAuthorizationRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.DocumentationRequirements)
            .WithOne()
            .HasForeignKey(e => e.PriorAuthorizationRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
