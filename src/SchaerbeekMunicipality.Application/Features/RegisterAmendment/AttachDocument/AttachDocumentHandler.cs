using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.AttachDocument;

public sealed record AttachDocumentResponse(
    Guid DocumentId,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed class AttachDocumentHandler(
    RegisterAmendmentCaseGuard caseGuard,
    IRegisterAmendmentCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<AttachDocumentResponse> Handle(
        RegisterAmendmentCaseId caseId,
        DocumentType documentType,
        string fileName,
        Stream fileContent,
        CancellationToken cancellationToken)
    {
        var amendmentCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(AttachDocument),
            cancellationToken);

        if (amendmentCase.Status != RegisterAmendmentCaseStatus.Draft)
            throw new InvalidRegisterAmendmentTransitionException(
                "Documents can only be attached while the case is in Draft status.");

        var stored = await documentStorage.SaveAsync(fileContent, fileName, cancellationToken);

        var document = AdministrativeDocument.CreateForRegisterAmendmentCase(
            caseId,
            documentType,
            fileName,
            stored.StoragePath,
            stored.ContentHash,
            OfficerId.From(currentOfficer.OfficerId),
            timeProvider.GetUtcNow());

        await documentRepository.AddAsync(document, cancellationToken);
        amendmentCase.MarkSupportingDocumentAttached();
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new AttachDocumentResponse(
            document.Id.Value,
            document.DocumentType,
            document.FileName,
            document.UploadedAt);
    }
}
