using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.AttachDocument;

public sealed record AttachDocumentResponse(
    Guid DocumentId,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed class AttachDocumentHandler(
    DeathDeclarationCaseGuard caseGuard,
    IDeathDeclarationCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<AttachDocumentResponse> Handle(
        DeathDeclarationCaseId caseId,
        string fileName,
        Stream fileContent,
        CancellationToken cancellationToken)
    {
        var deathDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(AttachDocument),
            cancellationToken);

        deathDeclarationCase.EnsureIntakeDataEditable(nameof(AttachDocument));

        var stored = await documentStorage.SaveAsync(fileContent, fileName, cancellationToken);

        var document = AdministrativeDocument.CreateForDeathDeclarationCase(
            caseId,
            DocumentType.DeathCertificate,
            fileName,
            stored.StoragePath,
            stored.ContentHash,
            OfficerId.From(currentOfficer.OfficerId),
            timeProvider.GetUtcNow());

        await documentRepository.AddAsync(document, cancellationToken);
        deathDeclarationCase.AttachDeathAct(document.Id);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new AttachDocumentResponse(
            document.Id.Value,
            document.DocumentType,
            document.FileName,
            document.UploadedAt);
    }
}
