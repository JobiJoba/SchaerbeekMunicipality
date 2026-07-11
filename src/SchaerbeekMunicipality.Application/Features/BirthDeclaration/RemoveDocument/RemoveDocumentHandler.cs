using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.RemoveDocument;

public sealed record RemoveDocumentResponse(Guid CaseId, Guid DocumentId, bool MedicalDeclarationAttached);

public sealed class RemoveDocumentHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage)
{
    public async Task<RemoveDocumentResponse> Handle(
        BirthDeclarationCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RemoveDocument),
            cancellationToken);

        birthDeclarationCase.EnsureIntakeDataEditable(nameof(RemoveDocument));

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (!document.BelongsToBirthDeclarationCase(caseId))
        {
            throw new InvalidBirthDeclarationTransitionException(
                "The document does not belong to this birth declaration case.");
        }

        documentRepository.Remove(document);
        await documentStorage.DeleteAsync(document.StoragePath, cancellationToken);
        birthDeclarationCase.RemoveMedicalDeclaration();
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RemoveDocumentResponse(
            birthDeclarationCase.Id.Value,
            documentId.Value,
            birthDeclarationCase.Checklist.MedicalDeclarationAttached);
    }
}
