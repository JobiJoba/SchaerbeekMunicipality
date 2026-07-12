using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ResolveRegisteredPerson;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.ResolveRegisteredPerson;

public static class ResolveRegisteredPersonEndpoint
{
    public static async Task<IResult> Handle(
        ResolveRegisteredPersonRequest request,
        ResolveRegisteredPersonHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
