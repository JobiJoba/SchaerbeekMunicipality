using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.AttachApplicantPhoto;

public sealed record AttachApplicantPhotoResponse(
    Guid DocumentId,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt,
    bool PhotoAttached);

public sealed class AttachApplicantPhotoHandler(
    DocumentRequestCaseGuard caseGuard,
    IDocumentRequestCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<AttachApplicantPhotoResponse> Handle(
        DocumentRequestCaseId caseId,
        string fileName,
        Stream fileContent,
        CancellationToken cancellationToken)
    {
        var documentRequestCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(AttachApplicantPhoto),
            cancellationToken);

        var stored = await documentStorage.SaveAsync(fileContent, fileName, cancellationToken);

        var document = AdministrativeDocument.CreateForDocumentRequestCase(
            caseId,
            DocumentType.ApplicantPhoto,
            fileName,
            stored.StoragePath,
            stored.ContentHash,
            OfficerId.From(currentOfficer.OfficerId),
            timeProvider.GetUtcNow());

        await documentRepository.AddAsync(document, cancellationToken);
        documentRequestCase.AttachApplicantPhoto(document.Id);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new AttachApplicantPhotoResponse(
            document.Id.Value,
            document.DocumentType,
            document.FileName,
            document.UploadedAt,
            documentRequestCase.PhotoAttached);
    }
}
