using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.DownloadDocument;

public sealed record DownloadDocumentResult(Stream Content, string FileName, string ContentType);

public sealed class DownloadDocumentHandler(
    IRegistrationCaseRepository caseRepository,
    IAdministrativeDocumentRepository documentRepository,
    IDocumentStorage documentStorage)
{
    public async Task<DownloadDocumentResult> Handle(
        RegistrationCaseId caseId,
        AdministrativeDocumentId documentId,
        CancellationToken cancellationToken)
    {
        _ = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Document '{documentId}' was not found.");

        if (document.RegistrationCaseId != caseId)
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
