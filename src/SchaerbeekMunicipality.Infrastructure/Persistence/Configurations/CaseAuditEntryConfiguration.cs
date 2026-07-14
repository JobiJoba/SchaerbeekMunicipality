using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class CaseAuditEntryConfiguration : IEntityTypeConfiguration<CaseAuditEntry>
{
    public void Configure(EntityTypeBuilder<CaseAuditEntry> builder)
    {
        builder.ToTable("case_audit_entries");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => CaseAuditEntryId.From(value))
            .HasColumnName("id");

        builder.Property(e => e.CaseId)
            .HasConversion(
                id => id.Value,
                value => new RegistrationCaseId(value))
            .HasColumnName("case_id");

        builder.Property(e => e.Action)
            .HasColumnName("action")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(e => e.OfficerId)
            .HasConversion(
                id => id.Value,
                value => OfficerId.From(value))
            .HasColumnName("officer_id");

        builder.Property(e => e.OccurredAt)
            .HasColumnName("occurred_at");

        builder.Property(e => e.Details)
            .HasColumnName("details")
            .HasMaxLength(2000);

        builder.HasIndex(e => e.CaseId);
    }
}