using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
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

    public DeathDeclarationCaseId? DeathDeclarationCaseId { get; private set; }

    public DocumentRequestCaseId? DocumentRequestCaseId { get; private set; }

    public RegisterAmendmentCaseId? RegisterAmendmentCaseId { get; private set; }

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
            UploadedAt = uploadedAt
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
            throw new ArgumentException(
                "Birth declaration cases only accept medical birth declaration documents.");

        return new AdministrativeDocument
        {
            Id = AdministrativeDocumentId.New(),
            BirthDeclarationCaseId = birthDeclarationCaseId,
            DocumentType = documentType,
            FileName = fileName.Trim(),
            StoragePath = storagePath,
            ContentHash = contentHash,
            UploadedByOfficerId = uploadedByOfficerId,
            UploadedAt = uploadedAt
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
            throw new ArgumentException(
                "Change of address cases only accept rental contracts or other housing documents.");

        return new AdministrativeDocument
        {
            Id = AdministrativeDocumentId.New(),
            ChangeOfAddressCaseId = changeOfAddressCaseId,
            DocumentType = documentType,
            FileName = fileName.Trim(),
            StoragePath = storagePath,
            ContentHash = contentHash,
            UploadedByOfficerId = uploadedByOfficerId,
            UploadedAt = uploadedAt
        };
    }

    public bool BelongsToRegistrationCase(RegistrationCaseId caseId)
    {
        return RegistrationCaseId == caseId;
    }

    public bool BelongsToBirthDeclarationCase(BirthDeclarationCaseId caseId)
    {
        return BirthDeclarationCaseId == caseId;
    }

    public static AdministrativeDocument CreateForDocumentRequestCase(
        DocumentRequestCaseId documentRequestCaseId,
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

        if (documentType != DocumentType.ApplicantPhoto)
            throw new ArgumentException(
                "Document request cases only accept applicant photo documents.");

        return new AdministrativeDocument
        {
            Id = AdministrativeDocumentId.New(),
            DocumentRequestCaseId = documentRequestCaseId,
            DocumentType = documentType,
            FileName = fileName.Trim(),
            StoragePath = storagePath,
            ContentHash = contentHash,
            UploadedByOfficerId = uploadedByOfficerId,
            UploadedAt = uploadedAt
        };
    }

    public bool BelongsToChangeOfAddressCase(ChangeOfAddressCaseId caseId)
    {
        return ChangeOfAddressCaseId == caseId;
    }

    public static AdministrativeDocument CreateForDeathDeclarationCase(
        DeathDeclarationCaseId deathDeclarationCaseId,
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

        if (documentType != DocumentType.DeathCertificate)
            throw new ArgumentException(
                "Death declaration cases only accept death certificate documents.");

        return new AdministrativeDocument
        {
            Id = AdministrativeDocumentId.New(),
            DeathDeclarationCaseId = deathDeclarationCaseId,
            DocumentType = documentType,
            FileName = fileName.Trim(),
            StoragePath = storagePath,
            ContentHash = contentHash,
            UploadedByOfficerId = uploadedByOfficerId,
            UploadedAt = uploadedAt
        };
    }

    public bool BelongsToDeathDeclarationCase(DeathDeclarationCaseId caseId)
    {
        return DeathDeclarationCaseId == caseId;
    }

    public bool BelongsToDocumentRequestCase(DocumentRequestCaseId caseId)
    {
        return DocumentRequestCaseId == caseId;
    }

    public static AdministrativeDocument CreateForRegisterAmendmentCase(
        RegisterAmendmentCaseId registerAmendmentCaseId,
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

        if (documentType is not (DocumentType.BirthCertificate or DocumentType.Other))
            throw new ArgumentException(
                "Register amendment cases only accept birth certificates or other supporting documents.");

        return new AdministrativeDocument
        {
            Id = AdministrativeDocumentId.New(),
            RegisterAmendmentCaseId = registerAmendmentCaseId,
            DocumentType = documentType,
            FileName = fileName.Trim(),
            StoragePath = storagePath,
            ContentHash = contentHash,
            UploadedByOfficerId = uploadedByOfficerId,
            UploadedAt = uploadedAt
        };
    }

    public bool BelongsToRegisterAmendmentCase(RegisterAmendmentCaseId caseId)
    {
        return RegisterAmendmentCaseId == caseId;
    }
}