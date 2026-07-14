using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.AttachDocument;

public sealed record AttachDocumentResponse(
    Guid DocumentId,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed class AttachDocumentHandler(
    ChangeOfAddressCaseGuard caseGuard,
    IChangeOfAddressCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<AttachDocumentResponse> Handle(
        ChangeOfAddressCaseId caseId,
        string fileName,
        Stream fileContent,
        CancellationToken cancellationToken)
    {
        var changeOfAddressCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(AttachDocument),
            cancellationToken);

        changeOfAddressCase.EnsureIntakeDataEditable(nameof(AttachDocument));

        var stored = await documentStorage.SaveAsync(fileContent, fileName, cancellationToken);

        var document = AdministrativeDocument.CreateForChangeOfAddressCase(
            caseId,
            DocumentType.RentalContract,
            fileName,
            stored.StoragePath,
            stored.ContentHash,
            OfficerId.From(currentOfficer.OfficerId),
            timeProvider.GetUtcNow());

        await documentRepository.AddAsync(document, cancellationToken);
        changeOfAddressCase.AttachHousingDocument(document.Id);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new AttachDocumentResponse(
            document.Id.Value,
            document.DocumentType,
            document.FileName,
            document.UploadedAt);
    }
}