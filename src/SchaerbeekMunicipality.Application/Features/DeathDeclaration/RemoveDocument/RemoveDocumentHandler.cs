using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.RemoveDocument;

public sealed record RemoveDocumentResponse(Guid CaseId, Guid DocumentId, bool DeathActAttached);

public sealed class RemoveDocumentHandler(
    DeathDeclarationCaseGuard caseGuard,
    IDeathDeclarationCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage)
{
    public async Task<RemoveDocumentResponse> Handle(
        DeathDeclarationCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        var deathDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RemoveDocument),
            cancellationToken);

        deathDeclarationCase.EnsureIntakeDataEditable(nameof(RemoveDocument));

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
                       ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (!document.BelongsToDeathDeclarationCase(caseId))
            throw new InvalidDeathDeclarationTransitionException(
                "The document does not belong to this death declaration case.");

        documentRepository.Remove(document);
        await documentStorage.DeleteAsync(document.StoragePath, cancellationToken);
        deathDeclarationCase.RemoveDeathAct();
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RemoveDocumentResponse(
            deathDeclarationCase.Id.Value,
            documentId.Value,
            deathDeclarationCase.Checklist.DeathActAttached);
    }
}
