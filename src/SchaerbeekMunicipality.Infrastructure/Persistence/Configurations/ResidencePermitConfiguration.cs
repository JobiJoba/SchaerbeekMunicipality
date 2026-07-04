using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class ResidencePermitConfiguration : IEntityTypeConfiguration<ResidencePermit>
{
    public void Configure(EntityTypeBuilder<ResidencePermit> builder)
    {
        builder.ToTable("residence_permits");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => ResidencePermitId.From(value))
            .HasColumnName("id");

        builder.Property(p => p.RegistrationCaseId)
            .HasConversion(
                id => id.Value,
                value => new RegistrationCaseId(value))
            .HasColumnName("registration_case_id");

        builder.HasIndex(p => p.RegistrationCaseId)
            .IsUnique();

        builder.Property(p => p.PermitType)
            .HasColumnName("permit_type")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(p => p.CardNumber)
            .HasColumnName("card_number")
            .HasMaxLength(64);

        builder.Property(p => p.ValidFrom)
            .HasColumnName("valid_from");

        builder.Property(p => p.ValidUntil)
            .HasColumnName("valid_until");

        builder.Property(p => p.IssuingAuthority)
            .HasColumnName("issuing_authority")
            .HasMaxLength(128);

        builder.Property(p => p.RecordedAt)
            .HasColumnName("recorded_at");
    }
}
