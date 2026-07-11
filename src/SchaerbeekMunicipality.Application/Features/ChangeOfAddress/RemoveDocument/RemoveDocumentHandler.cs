using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.RemoveDocument;

public sealed record RemoveDocumentResponse(Guid CaseId, Guid DocumentId, bool HousingDocumentAttached);

public sealed class RemoveDocumentHandler(
    ChangeOfAddressCaseGuard caseGuard,
    IChangeOfAddressCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage)
{
    public async Task<RemoveDocumentResponse> Handle(
        ChangeOfAddressCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        var changeOfAddressCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RemoveDocument),
            cancellationToken);

        changeOfAddressCase.EnsureIntakeDataEditable(nameof(RemoveDocument));

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (!document.BelongsToChangeOfAddressCase(caseId))
        {
            throw new InvalidChangeOfAddressTransitionException(
                "The document does not belong to this change of address case.");
        }

        documentRepository.Remove(document);
        await documentStorage.DeleteAsync(document.StoragePath, cancellationToken);
        changeOfAddressCase.RemoveHousingDocument();
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RemoveDocumentResponse(
            changeOfAddressCase.Id.Value,
            documentId.Value,
            changeOfAddressCase.Checklist.HousingDocumentAttached);
    }
}
