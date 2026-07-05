using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
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

        builder.Property(c => c.ResidenceCategory)
            .HasColumnName("residence_category")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.OpenedAt)
            .HasColumnName("opened_at");

        builder.Property(c => c.ClosedAt)
            .HasColumnName("closed_at");

        builder.Property(c => c.SelectedRegisterTarget)
            .HasColumnName("register_target")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.DecisionOfficerId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? OfficerId.From(value.Value) : null)
            .HasColumnName("decision_officer_id");

        builder.Property(c => c.RejectionReason)
            .HasColumnName("rejection_reason")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.SuspensionReason)
            .HasColumnName("suspension_reason")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.DecisionNotes)
            .HasColumnName("decision_notes")
            .HasMaxLength(2000);

        builder.Property(c => c.StatusBeforeSuspension)
            .HasColumnName("status_before_suspension")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.OwnsOne(c => c.Checklist, checklist =>
        {
            checklist.Property(c => c.IdentityEstablished).HasColumnName("identity_established");
            checklist.Property(c => c.LegalResidenceEstablished).HasColumnName("legal_residence_established");
            checklist.Property(c => c.AddressDeclared).HasColumnName("address_declared");
            checklist.Property(c => c.AddressConfirmed).HasColumnName("address_confirmed");
            checklist.Property(c => c.RegisterDeterminable).HasColumnName("register_determinable");
        });

        builder.OwnsOne(c => c.ImmigrationDecision, decision =>
        {
            decision.Property(d => d.ReferenceNumber).HasColumnName("immigration_decision_reference").HasMaxLength(128);
            decision.Property(d => d.DecisionDate).HasColumnName("immigration_decision_date");
        });

        builder.OwnsOne(c => c.DeclaredAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("address_street").HasMaxLength(256);
            address.Property(a => a.HouseNumber).HasColumnName("address_house_number").HasMaxLength(16);
            address.Property(a => a.Box).HasColumnName("address_box").HasMaxLength(16);
            address.Property(a => a.PostalCode).HasColumnName("address_postal_code").HasMaxLength(4);
            address.Property(a => a.Municipality).HasColumnName("address_municipality").HasMaxLength(128);
        });

        builder.Property(c => c.HousingSituation)
            .HasColumnName("housing_situation")
            .HasConversion<string>()
            .HasMaxLength(64);
    }
}
