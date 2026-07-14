using SchaerbeekMunicipality.Application.Features.BirthDeclaration.DownloadDocument;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.DownloadDocument;

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
                new BirthDeclarationCaseId(id),
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