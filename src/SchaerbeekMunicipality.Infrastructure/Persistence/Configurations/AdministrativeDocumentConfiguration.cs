using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class AdministrativeDocumentConfiguration : IEntityTypeConfiguration<AdministrativeDocument>
{
    public void Configure(EntityTypeBuilder<AdministrativeDocument> builder)
    {
        builder.ToTable("administrative_documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => new AdministrativeDocumentId(value))
            .HasColumnName("id");

        builder.Property(d => d.RegistrationCaseId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new RegistrationCaseId(value.Value) : null)
            .HasColumnName("registration_case_id");

        builder.Property(d => d.BirthDeclarationCaseId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new BirthDeclarationCaseId(value.Value) : null)
            .HasColumnName("birth_declaration_case_id");

        builder.Property(d => d.ChangeOfAddressCaseId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new ChangeOfAddressCaseId(value.Value) : null)
            .HasColumnName("change_of_address_case_id");

        builder.Property(d => d.DocumentRequestCaseId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new DocumentRequestCaseId(value.Value) : null)
            .HasColumnName("document_request_case_id");

        builder.Property(d => d.DocumentType)
            .HasColumnName("document_type")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(d => d.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(d => d.StoragePath)
            .HasColumnName("storage_path")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(d => d.ContentHash)
            .HasColumnName("content_hash")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(d => d.UploadedByOfficerId)
            .HasConversion(
                id => id.Value,
                value => OfficerId.From(value))
            .HasColumnName("uploaded_by_officer_id");

        builder.Property(d => d.UploadedAt)
            .HasColumnName("uploaded_at");

        builder.HasIndex(d => d.RegistrationCaseId);
        builder.HasIndex(d => d.BirthDeclarationCaseId);
        builder.HasIndex(d => d.ChangeOfAddressCaseId);
        builder.HasIndex(d => d.DocumentRequestCaseId);
    }
}
