using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.DownloadDocument;

public sealed record DownloadDocumentResult(Stream Content, string FileName, string ContentType);

public sealed class DownloadDocumentHandler(
    ChangeOfAddressCaseGuard caseGuard,
    ChangeOfAddressCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage)
{
    public async Task<DownloadDocumentResult> Handle(
        ChangeOfAddressCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);
        _ = await caseGuard.GetForViewAsync(caseId, cancellationToken);

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
                       ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (!document.BelongsToChangeOfAddressCase(caseId))
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