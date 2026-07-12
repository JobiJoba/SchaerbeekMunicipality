using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.DownloadDocument;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.DownloadDocument;

public static class DownloadDocumentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        Guid documentId,
        DownloadDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            DocumentRequestCaseId.From(id),
            new AdministrativeDocumentId(documentId),
            cancellationToken);

        return Results.File(result.Content, result.ContentType, result.FileName);
    }
}
