using SchaerbeekMunicipality.Application.Features.IdentityDocuments.RemoveDocument;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.RemoveDocument;

public static class RemoveDocumentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        Guid documentId,
        RemoveDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Handle(
            DocumentRequestCaseId.From(id),
            new AdministrativeDocumentId(documentId),
            cancellationToken);

        return Results.Ok(response);
    }
}