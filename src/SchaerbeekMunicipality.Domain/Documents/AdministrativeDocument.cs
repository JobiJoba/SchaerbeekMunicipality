using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Documents;

public sealed class AdministrativeDocument
{
    private AdministrativeDocument()
    {
    }

    public AdministrativeDocumentId Id { get; private set; }

    public RegistrationCaseId? RegistrationCaseId { get; private set; }

    public BirthDeclarationCaseId? BirthDeclarationCaseId { get; private set; }

    public ChangeOfAddressCaseId? ChangeOfAddressCaseId { get; private set; }

    public DocumentType DocumentType { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string StoragePath { get; private set; } = string.Empty;

    public string ContentHash { get; private set; } = string.Empty;

    public OfficerId UploadedByOfficerId { get; private set; }

    public DateTimeOffset UploadedAt { get; private set; }

    public static AdministrativeDocument CreateForRegistrationCase(
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

    public static AdministrativeDocument CreateForBirthDeclarationCase(
        BirthDeclarationCaseId birthDeclarationCaseId,
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

        if (documentType != DocumentType.MedicalBirthDeclaration)
        {
            throw new ArgumentException(
                "Birth declaration cases only accept medical birth declaration documents.");
        }

        return new AdministrativeDocument
        {
            Id = AdministrativeDocumentId.New(),
            BirthDeclarationCaseId = birthDeclarationCaseId,
            DocumentType = documentType,
            FileName = fileName.Trim(),
            StoragePath = storagePath,
            ContentHash = contentHash,
            UploadedByOfficerId = uploadedByOfficerId,
            UploadedAt = uploadedAt,
        };
    }

    public static AdministrativeDocument CreateForChangeOfAddressCase(
        ChangeOfAddressCaseId changeOfAddressCaseId,
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

        if (documentType is not (DocumentType.RentalContract or DocumentType.Other))
        {
            throw new ArgumentException(
                "Change of address cases only accept rental contracts or other housing documents.");
        }

        return new AdministrativeDocument
        {
            Id = AdministrativeDocumentId.New(),
            ChangeOfAddressCaseId = changeOfAddressCaseId,
            DocumentType = documentType,
            FileName = fileName.Trim(),
            StoragePath = storagePath,
            ContentHash = contentHash,
            UploadedByOfficerId = uploadedByOfficerId,
            UploadedAt = uploadedAt,
        };
    }

    public bool BelongsToRegistrationCase(RegistrationCaseId caseId) =>
        RegistrationCaseId == caseId;

    public bool BelongsToBirthDeclarationCase(BirthDeclarationCaseId caseId) =>
        BirthDeclarationCaseId == caseId;

    public bool BelongsToChangeOfAddressCase(ChangeOfAddressCaseId caseId) =>
        ChangeOfAddressCaseId == caseId;
}
