using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.DownloadDocument;

public sealed record DownloadDocumentResult(Stream Content, string FileName, string ContentType);

public sealed class DownloadDocumentHandler(
    BirthDeclarationCaseGuard caseGuard,
    BirthDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage)
{
    public async Task<DownloadDocumentResult> Handle(
        BirthDeclarationCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);
        _ = await caseGuard.GetForViewAsync(caseId, cancellationToken);

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (!document.BelongsToBirthDeclarationCase(caseId))
        {
            throw new KeyNotFoundException($"Document '{documentId}' was not found on case '{caseId}'.");
        }

        var content = await documentStorage.OpenReadAsync(document.StoragePath, cancellationToken);

        return new DownloadDocumentResult(
            content,
            document.FileName,
            GetContentType(document.FileName));
    }

    private static string GetContentType(string fileName) =>
        Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream",
        };
}
