using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class DocumentRequestCaseConfiguration : IEntityTypeConfiguration<DocumentRequestCase>
{
    public void Configure(EntityTypeBuilder<DocumentRequestCase> builder)
    {
        builder.ToTable("document_request_cases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => DocumentRequestCaseId.From(value))
            .HasColumnName("id");

        builder.Property(c => c.PersonId)
            .HasConversion(
                id => id.Value,
                value => new PersonId(value))
            .HasColumnName("person_id");

        builder.Property(c => c.RequestType)
            .HasColumnName("request_type")
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

        builder.Property(c => c.PhotoDocumentId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new AdministrativeDocumentId(value.Value) : null)
            .HasColumnName("photo_document_id");

        builder.Property(c => c.PhotoAttached)
            .HasColumnName("photo_attached");

        builder.Property(c => c.FeePaid)
            .HasColumnName("fee_paid");

        builder.Property(c => c.FeePaymentReference)
            .HasColumnName("fee_payment_reference")
            .HasMaxLength(128);

        builder.Property(c => c.IssuedDocumentNumber)
            .HasColumnName("issued_document_number")
            .HasMaxLength(64);

        builder.Property(c => c.RequestedAt)
            .HasColumnName("requested_at");

        builder.Property(c => c.IssuedAt)
            .HasColumnName("issued_at");

        builder.Property(c => c.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.Property(c => c.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(2000);
    }
}