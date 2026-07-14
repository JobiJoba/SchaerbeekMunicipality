using SchaerbeekMunicipality.Application.Features.IdentityDocuments.IssueDocument;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.IssueDocument;

public static class IssueDocumentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IssueDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Handle(DocumentRequestCaseId.From(id), cancellationToken);
        return Results.Ok(response);
    }
}