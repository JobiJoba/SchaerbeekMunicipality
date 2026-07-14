using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ClaimDocumentRequestCase;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.ClaimDocumentRequestCase;

public static class AutoClaimDocumentRequestCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimDocumentRequestCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.TryAutoClaimAsync(DocumentRequestCaseId.From(id), cancellationToken);
        return response is null ? Results.NoContent() : Results.Ok(response);
    }
}