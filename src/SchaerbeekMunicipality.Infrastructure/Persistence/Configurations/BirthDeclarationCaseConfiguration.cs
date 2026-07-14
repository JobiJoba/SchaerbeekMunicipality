using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class BirthDeclarationCaseConfiguration : IEntityTypeConfiguration<BirthDeclarationCase>
{
    public void Configure(EntityTypeBuilder<BirthDeclarationCase> builder)
    {
        builder.ToTable("birth_declaration_cases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => new BirthDeclarationCaseId(value))
            .HasColumnName("id");

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

        builder.Property(c => c.ChildGivenNames)
            .HasColumnName("child_given_names")
            .HasMaxLength(256);

        builder.Property(c => c.ChildFamilyName)
            .HasColumnName("child_family_name")
            .HasMaxLength(256);

        builder.Property(c => c.ChildSex)
            .HasColumnName("child_sex")
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(c => c.ChildDateOfBirth)
            .HasColumnName("child_date_of_birth");

        builder.Property(c => c.ChildTimeOfBirth)
            .HasColumnName("child_time_of_birth");

        builder.Property(c => c.ChildPlaceOfBirth)
            .HasColumnName("child_place_of_birth")
            .HasMaxLength(256);

        builder.Property(c => c.MedicalDeclarationDocumentId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new AdministrativeDocumentId(value.Value) : null)
            .HasColumnName("medical_declaration_document_id");

        builder.Property(c => c.OpenedAt)
            .HasColumnName("opened_at");

        builder.Property(c => c.ConfirmedAt)
            .HasColumnName("confirmed_at");

        builder.Property(c => c.ClosedAt)
            .HasColumnName("closed_at");

        builder.Property(c => c.ChildPersonId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new PersonId(value.Value) : null)
            .HasColumnName("child_person_id");

        builder.Property(c => c.ChildNationalRegisterNumber)
            .HasColumnName("child_national_register_number")
            .HasMaxLength(32);

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
            checklist.Property(c => c.ChildDetailsRecorded).HasColumnName("child_details_recorded");
            checklist.Property(c => c.AtLeastOneParentLinked).HasColumnName("at_least_one_parent_linked");
            checklist.Property(c => c.MedicalDeclarationAttached).HasColumnName("medical_declaration_attached");
            checklist.Property(c => c.HouseholdEstablished).HasColumnName("household_established");
        });

        builder.OwnsOne(c => c.HouseholdAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("household_street").HasMaxLength(256);
            address.Property(a => a.HouseNumber).HasColumnName("household_house_number").HasMaxLength(16);
            address.Property(a => a.Box).HasColumnName("household_box").HasMaxLength(16);
            address.Property(a => a.PostalCode).HasColumnName("household_postal_code").HasMaxLength(4);
            address.Property(a => a.Municipality).HasColumnName("household_municipality").HasMaxLength(128);
        });

        builder.OwnsMany(c => c.ParentLinks, links =>
        {
            links.ToTable("birth_declaration_parent_links");
            links.WithOwner().HasForeignKey("BirthDeclarationCaseId");
            links.Property<BirthDeclarationCaseId>("BirthDeclarationCaseId")
                .HasConversion(
                    id => id.Value,
                    value => new BirthDeclarationCaseId(value))
                .HasColumnName("birth_declaration_case_id");
            links.HasKey("BirthDeclarationCaseId", nameof(BirthDeclarationParentLink.PersonId));
            links.Property(l => l.PersonId)
                .HasConversion(
                    id => id.Value,
                    value => new PersonId(value))
                .HasColumnName("person_id");
            links.Property(l => l.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .HasMaxLength(32);
        });

        builder.Navigation(c => c.ParentLinks).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}