using SchaerbeekMunicipality.Application.Features.Registration.DownloadDocument;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;

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
                result.ContentType,
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