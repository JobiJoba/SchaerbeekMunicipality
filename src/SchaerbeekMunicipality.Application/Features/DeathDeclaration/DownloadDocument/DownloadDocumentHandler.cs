using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.DownloadDocument;

public sealed record DownloadDocumentResult(Stream Content, string FileName, string ContentType);

public sealed class DownloadDocumentHandler(
    DeathDeclarationCaseGuard caseGuard,
    DeathDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage)
{
    public async Task<DownloadDocumentResult> Handle(
        DeathDeclarationCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);
        _ = await caseGuard.GetForViewAsync(caseId, cancellationToken);

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
                       ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (!document.BelongsToDeathDeclarationCase(caseId))
            throw new KeyNotFoundException($"Document '{documentId}' was not found on case '{caseId}'.");

        var content = await documentStorage.OpenReadAsync(document.StoragePath, cancellationToken);

        return new DownloadDocumentResult(
            content,
            document.FileName,
            GetContentType(document.FileName));
    }

    private static string GetContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}
