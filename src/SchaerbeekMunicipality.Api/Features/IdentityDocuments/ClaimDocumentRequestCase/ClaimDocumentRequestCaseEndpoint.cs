using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ClaimDocumentRequestCase;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.ClaimDocumentRequestCase;

public static class ClaimDocumentRequestCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimDocumentRequestCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Handle(DocumentRequestCaseId.From(id), cancellationToken);
        return Results.Ok(response);
    }
}