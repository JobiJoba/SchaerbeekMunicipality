using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Certificates;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class CertificateRequestConfiguration : IEntityTypeConfiguration<CertificateRequest>
{
    public void Configure(EntityTypeBuilder<CertificateRequest> builder)
    {
        builder.ToTable("certificate_requests");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => CertificateRequestId.From(value))
            .HasColumnName("id");

        builder.Property(c => c.RegistrationCaseId)
            .HasConversion(
                id => id.Value,
                value => new RegistrationCaseId(value))
            .HasColumnName("registration_case_id");

        builder.Property(c => c.CertificateType)
            .HasColumnName("certificate_type")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.PersonId)
            .HasConversion(
                id => id.Value,
                value => new PersonId(value))
            .HasColumnName("person_id");

        builder.Property(c => c.IssuedByOfficerId)
            .HasConversion(
                id => id.Value,
                value => OfficerId.From(value))
            .HasColumnName("issued_by_officer_id");

        builder.Property(c => c.ReferenceNumber)
            .HasColumnName("reference_number")
            .HasMaxLength(64);

        builder.Property(c => c.IssuedAt)
            .HasColumnName("issued_at");

        builder.HasIndex(c => c.RegistrationCaseId);
        builder.HasIndex(c => c.ReferenceNumber).IsUnique();
    }
}