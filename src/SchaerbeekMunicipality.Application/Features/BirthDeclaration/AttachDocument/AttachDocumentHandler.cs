using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.AttachDocument;

public sealed record AttachDocumentResponse(
    Guid DocumentId,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed class AttachDocumentHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<AttachDocumentResponse> Handle(
        BirthDeclarationCaseId caseId,
        string fileName,
        Stream fileContent,
        CancellationToken cancellationToken)
    {
        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(AttachDocument),
            cancellationToken);

        birthDeclarationCase.EnsureIntakeDataEditable(nameof(AttachDocument));

        var stored = await documentStorage.SaveAsync(fileContent, fileName, cancellationToken);

        var document = AdministrativeDocument.CreateForBirthDeclarationCase(
            caseId,
            DocumentType.MedicalBirthDeclaration,
            fileName,
            stored.StoragePath,
            stored.ContentHash,
            OfficerId.From(currentOfficer.OfficerId),
            timeProvider.GetUtcNow());

        await documentRepository.AddAsync(document, cancellationToken);
        birthDeclarationCase.AttachMedicalDeclaration(document.Id);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new AttachDocumentResponse(
            document.Id.Value,
            document.DocumentType,
            document.FileName,
            document.UploadedAt);
    }
}