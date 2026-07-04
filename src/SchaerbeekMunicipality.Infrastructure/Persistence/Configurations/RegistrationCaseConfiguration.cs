using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class RegistrationCaseConfiguration : IEntityTypeConfiguration<RegistrationCase>
{
    public void Configure(EntityTypeBuilder<RegistrationCase> builder)
    {
        builder.ToTable("registration_cases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => new RegistrationCaseId(value))
            .HasColumnName("id");

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.VisitReason)
            .HasColumnName("visit_reason")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.AssignedOfficerId)
            .HasConversion(
                id => id.Value,
                value => OfficerId.From(value))
            .HasColumnName("assigned_officer_id");

        builder.Property(c => c.PersonId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new PersonId(value.Value) : null)
            .HasColumnName("person_id");

        builder.Property(c => c.OpenedAt)
            .HasColumnName("opened_at");

        builder.OwnsOne(c => c.Checklist, checklist =>
        {
            checklist.Property(c => c.IdentityEstablished).HasColumnName("identity_established");
            checklist.Property(c => c.LegalResidenceEstablished).HasColumnName("legal_residence_established");
            checklist.Property(c => c.AddressDeclared).HasColumnName("address_declared");
            checklist.Property(c => c.AddressConfirmed).HasColumnName("address_confirmed");
            checklist.Property(c => c.RegisterDeterminable).HasColumnName("register_determinable");
        });
    }
}
