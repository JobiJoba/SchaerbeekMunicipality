using SchaerbeekMunicipality.Application.Features.RegisterAmendment.DownloadDocument;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.DownloadDocument;

public static class DownloadDocumentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        Guid documentId,
        DownloadDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            RegisterAmendmentCaseId.From(id),
            new AdministrativeDocumentId(documentId),
            cancellationToken);

        return Results.File(result.Content, result.ContentType, result.FileName);
    }
}
