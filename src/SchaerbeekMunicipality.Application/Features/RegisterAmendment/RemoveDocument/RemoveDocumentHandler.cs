using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.RemoveDocument;

public sealed class RemoveDocumentHandler(
    RegisterAmendmentCaseGuard caseGuard,
    IRegisterAmendmentCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage)
{
    public async Task Handle(
        RegisterAmendmentCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        var amendmentCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RemoveDocument),
            cancellationToken);

        if (amendmentCase.Status != RegisterAmendmentCaseStatus.Draft)
            throw new InvalidRegisterAmendmentTransitionException(
                "Documents can only be removed while the case is in Draft status.");

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
                       ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (!document.BelongsToRegisterAmendmentCase(caseId))
            throw new InvalidRegisterAmendmentTransitionException(
                "The document does not belong to this register amendment case.");

        var existingDocuments = await documentRepository.ListByRegisterAmendmentCaseIdAsync(
            caseId,
            cancellationToken);
        var isLastDocument = existingDocuments.Count == 1 && existingDocuments[0].Id == documentId;

        documentRepository.Remove(document);
        await documentStorage.DeleteAsync(document.StoragePath, cancellationToken);

        if (isLastDocument)
            amendmentCase.ClearSupportingDocumentAttached();

        await caseRepository.SaveChangesAsync(cancellationToken);
    }
}
