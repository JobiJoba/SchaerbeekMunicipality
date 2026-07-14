using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class RegisterAmendmentCaseConfiguration : IEntityTypeConfiguration<RegisterAmendmentCase>
{
    public void Configure(EntityTypeBuilder<RegisterAmendmentCase> builder)
    {
        builder.ToTable("register_amendment_cases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => RegisterAmendmentCaseId.From(value))
            .HasColumnName("id");

        builder.Property(c => c.PersonId)
            .HasConversion(
                id => id.Value,
                value => new PersonId(value))
            .HasColumnName("person_id");

        builder.Property(c => c.AmendmentType)
            .HasColumnName("amendment_type")
            .HasConversion<string>()
            .HasMaxLength(64);

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

        builder.Property(c => c.Reason)
            .HasColumnName("reason")
            .HasMaxLength(2000);

        builder.Property(c => c.ProposedGivenName)
            .HasColumnName("proposed_given_name")
            .HasMaxLength(128);

        builder.Property(c => c.ProposedFamilyName)
            .HasColumnName("proposed_family_name")
            .HasMaxLength(128);

        builder.Property(c => c.ProposedNationality)
            .HasColumnName("proposed_nationality")
            .HasMaxLength(64);

        builder.Property(c => c.OpenedAt)
            .HasColumnName("opened_at");

        builder.Property(c => c.SubmittedAt)
            .HasColumnName("submitted_at");

        builder.Property(c => c.ApprovedAt)
            .HasColumnName("approved_at");

        builder.Property(c => c.AppliedAt)
            .HasColumnName("applied_at");

        builder.Property(c => c.ClosedAt)
            .HasColumnName("closed_at");

        builder.Property(c => c.DecisionOfficerId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? OfficerId.From(value.Value) : null)
            .HasColumnName("decision_officer_id");

        builder.Property(c => c.AppliedByOfficerId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? OfficerId.From(value.Value) : null)
            .HasColumnName("applied_by_officer_id");

        builder.Property(c => c.RejectionReason)
            .HasColumnName("rejection_reason")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(c => c.DecisionNotes)
            .HasColumnName("decision_notes")
            .HasMaxLength(2000);

        builder.OwnsOne(c => c.Checklist, checklist =>
        {
            checklist.Property(c => c.ProposedChangesRecorded).HasColumnName("proposed_changes_recorded");
            checklist.Property(c => c.SupportingDocumentAttached).HasColumnName("supporting_document_attached");
        });

        builder.OwnsOne(c => c.ProposedCivilStatus, civilStatus =>
        {
            civilStatus.Property(c => c.Status)
                .HasColumnName("proposed_civil_status")
                .HasConversion<string>()
                .HasMaxLength(32);

            civilStatus.Property(c => c.SpouseGivenName)
                .HasColumnName("proposed_spouse_given_name")
                .HasMaxLength(128);

            civilStatus.Property(c => c.SpouseFamilyName)
                .HasColumnName("proposed_spouse_family_name")
                .HasMaxLength(128);

            civilStatus.Property(c => c.MarriageDate)
                .HasColumnName("proposed_marriage_date");

            civilStatus.Property(c => c.MarriagePlace)
                .HasColumnName("proposed_marriage_place")
                .HasMaxLength(128);

            civilStatus.Property(c => c.MarriageRecognitionStatus)
                .HasColumnName("proposed_marriage_recognition_status")
                .HasConversion<string>()
                .HasMaxLength(32);
        });

        builder.HasIndex(c => c.PersonId);
        builder.HasIndex(c => c.Status);
    }
}
