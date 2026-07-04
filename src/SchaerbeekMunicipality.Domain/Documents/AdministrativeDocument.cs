using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Documents;

public sealed class AdministrativeDocument
{
    private AdministrativeDocument()
    {
    }

    public AdministrativeDocumentId Id { get; private set; }

    public RegistrationCaseId RegistrationCaseId { get; private set; }

    public DocumentType DocumentType { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string StoragePath { get; private set; } = string.Empty;

    public string ContentHash { get; private set; } = string.Empty;

    public OfficerId UploadedByOfficerId { get; private set; }

    public DateTimeOffset UploadedAt { get; private set; }

    public static AdministrativeDocument Create(
        RegistrationCaseId registrationCaseId,
        DocumentType documentType,
        string fileName,
        string storagePath,
        string contentHash,
        OfficerId uploadedByOfficerId,
        DateTimeOffset uploadedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(storagePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentHash);

        return new AdministrativeDocument
        {
            Id = AdministrativeDocumentId.New(),
            RegistrationCaseId = registrationCaseId,
            DocumentType = documentType,
            FileName = fileName.Trim(),
            StoragePath = storagePath,
            ContentHash = contentHash,
            UploadedByOfficerId = uploadedByOfficerId,
            UploadedAt = uploadedAt,
        };
    }
}
