using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class ChangeOfAddressCaseConfiguration : IEntityTypeConfiguration<ChangeOfAddressCase>
{
    public void Configure(EntityTypeBuilder<ChangeOfAddressCase> builder)
    {
        builder.ToTable("change_of_address_cases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => ChangeOfAddressCaseId.From(value))
            .HasColumnName("id");

        builder.Property(c => c.PersonId)
            .HasConversion(
                id => id.Value,
                value => new PersonId(value))
            .HasColumnName("person_id");

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.AssignedOfficerId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? OfficerId.From(value.Value) : null)
            .HasColumnName("assigned_officer_id");

        builder.Property(c => c.LockedByOfficerId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? OfficerId.From(value.Value) : null)
            .HasColumnName("locked_by_officer_id");

        builder.Property(c => c.LockedAt)
            .HasColumnName("locked_at");

        builder.Property(c => c.HousingSituation)
            .HasColumnName("housing_situation")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.HousingDocumentId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new AdministrativeDocumentId(value.Value) : null)
            .HasColumnName("housing_document_id");

        builder.Property(c => c.PoliceVerificationRequestId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? PoliceVerificationRequestId.From(value.Value) : null)
            .HasColumnName("police_verification_request_id");

        builder.Property(c => c.EffectiveDate)
            .HasColumnName("effective_date");

        builder.Property(c => c.OpenedAt)
            .HasColumnName("opened_at");

        builder.Property(c => c.ConfirmedAt)
            .HasColumnName("confirmed_at");

        builder.Property(c => c.ClosedAt)
            .HasColumnName("closed_at");

        builder.Property(c => c.DecisionOfficerId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? OfficerId.From(value.Value) : null)
            .HasColumnName("decision_officer_id");

        builder.Property(c => c.RejectionReason)
            .HasColumnName("rejection_reason")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.DecisionNotes)
            .HasColumnName("decision_notes")
            .HasMaxLength(2000);

        builder.OwnsOne(c => c.Checklist, checklist =>
        {
            checklist.Property(c => c.PersonIdentified).HasColumnName("person_identified");
            checklist.Property(c => c.NewAddressDeclared).HasColumnName("new_address_declared");
            checklist.Property(c => c.HousingDocumentAttached).HasColumnName("housing_document_attached");
            checklist.Property(c => c.HousingDocumentRequired).HasColumnName("housing_document_required");
            checklist.Property(c => c.PoliceVerificationRequested).HasColumnName("police_verification_requested");
            checklist.Property(c => c.PoliceVerificationPositive).HasColumnName("police_verification_positive");
        });

        builder.OwnsOne(c => c.PreviousAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("previous_street").HasMaxLength(256);
            address.Property(a => a.HouseNumber).HasColumnName("previous_house_number").HasMaxLength(16);
            address.Property(a => a.Box).HasColumnName("previous_box").HasMaxLength(16);
            address.Property(a => a.PostalCode).HasColumnName("previous_postal_code").HasMaxLength(4);
            address.Property(a => a.Municipality).HasColumnName("previous_municipality").HasMaxLength(128);
        });

        builder.OwnsOne(c => c.NewAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("new_street").HasMaxLength(256);
            address.Property(a => a.HouseNumber).HasColumnName("new_house_number").HasMaxLength(16);
            address.Property(a => a.Box).HasColumnName("new_box").HasMaxLength(16);
            address.Property(a => a.PostalCode).HasColumnName("new_postal_code").HasMaxLength(4);
            address.Property(a => a.Municipality).HasColumnName("new_municipality").HasMaxLength(128);
        });

        builder.OwnsMany(c => c.HouseholdMemberLinks, links =>
        {
            links.ToTable("change_of_address_household_member_links");
            links.WithOwner().HasForeignKey("ChangeOfAddressCaseId");
            links.Property<ChangeOfAddressCaseId>("ChangeOfAddressCaseId")
                .HasConversion(
                    id => id.Value,
                    value => ChangeOfAddressCaseId.From(value))
                .HasColumnName("change_of_address_case_id");
            links.HasKey("ChangeOfAddressCaseId", nameof(ChangeOfAddressHouseholdMemberLink.PersonId));
            links.Property(l => l.PersonId)
                .HasConversion(
                    id => id.Value,
                    value => new PersonId(value))
                .HasColumnName("person_id");
        });

        builder.Navigation(c => c.HouseholdMemberLinks).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}