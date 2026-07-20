using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class DeathDeclarationCaseConfiguration : IEntityTypeConfiguration<DeathDeclarationCase>
{
    public void Configure(EntityTypeBuilder<DeathDeclarationCase> builder)
    {
        builder.ToTable("death_declaration_cases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => new DeathDeclarationCaseId(value))
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

        builder.Property(c => c.DeathDate)
            .HasColumnName("death_date");

        builder.Property(c => c.DeathPlace)
            .HasColumnName("death_place")
            .HasMaxLength(256);

        builder.Property(c => c.DeathAbroad)
            .HasColumnName("death_abroad");

        builder.Property(c => c.InformantRelationship)
            .HasColumnName("informant_relationship")
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(c => c.DeathActDocumentId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new AdministrativeDocumentId(value.Value) : null)
            .HasColumnName("death_act_document_id");

        builder.Property(c => c.HouseholdReviewedAt)
            .HasColumnName("household_reviewed_at");

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
            checklist.Property(c => c.PersonIdentified).HasColumnName("person_identified");
            checklist.Property(c => c.DeathFactsRecorded).HasColumnName("death_facts_recorded");
            checklist.Property(c => c.DeathActAttached).HasColumnName("death_act_attached");
            checklist.Property(c => c.HouseholdReviewed).HasColumnName("household_reviewed");
        });

        builder.HasIndex(c => c.PersonId);
    }
}
