using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration.DownloadDocument;

namespace SchaerbeekMunicipality.Api.Features.Registration.DownloadDocument;

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
                new RegistrationCaseId(id),
                new AdministrativeDocumentId(documentId),
                cancellationToken);

            return Results.Stream(
                result.Content,
                contentType: result.ContentType,
                enableRangeProcessing: true);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (FileNotFoundException)
        {
            return Results.NotFound();
        }
    }
}
