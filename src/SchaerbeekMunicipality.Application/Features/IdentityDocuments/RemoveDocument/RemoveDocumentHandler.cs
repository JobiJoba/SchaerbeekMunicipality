using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.RemoveDocument;

public sealed record RemoveDocumentResponse(Guid CaseId, Guid DocumentId, bool PhotoAttached);

public sealed class RemoveDocumentHandler(
    DocumentRequestCaseGuard caseGuard,
    IDocumentRequestCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage)
{
    public async Task<RemoveDocumentResponse> Handle(
        DocumentRequestCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        var documentRequestCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RemoveDocument),
            cancellationToken);

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
                       ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (!document.BelongsToDocumentRequestCase(caseId))
            throw new InvalidDocumentRequestTransitionException(
                "The document does not belong to this document request case.");

        documentRepository.Remove(document);
        await documentStorage.DeleteAsync(document.StoragePath, cancellationToken);
        documentRequestCase.RemoveApplicantPhoto();
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RemoveDocumentResponse(
            documentRequestCase.Id.Value,
            documentId.Value,
            documentRequestCase.PhotoAttached);
    }
}