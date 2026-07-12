using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.GetDocumentRequestCase;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.GetDocumentRequestCase;

public static class GetDocumentRequestCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetDocumentRequestCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Handle(DocumentRequestCaseId.From(id), cancellationToken);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }
}
