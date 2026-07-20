using SchaerbeekMunicipality.Application.Features.DeathDeclaration.DownloadDocument;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.DownloadDocument;

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
                new DeathDeclarationCaseId(id),
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
