using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ReleaseCaseLock;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.ReleaseCaseLock;

public static class ReleaseCaseLockEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ReleaseCaseLockHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Handle(DocumentRequestCaseId.From(id), cancellationToken);
        return Results.Ok(response);
    }
}