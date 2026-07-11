using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.DownloadDocument;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.DownloadDocument;

public static class DownloadDocumentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        Guid documentId,
        DownloadDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(
                new ChangeOfAddressCaseId(id),
                new AdministrativeDocumentId(documentId),
                cancellationToken);

            return Results.File(result.Content, result.ContentType, result.FileName);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
    }
}
