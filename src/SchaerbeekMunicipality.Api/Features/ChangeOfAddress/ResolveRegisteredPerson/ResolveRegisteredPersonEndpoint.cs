using Microsoft.AspNetCore.Mvc;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ResolveRegisteredPerson;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ResolveRegisteredPerson;

public static class ResolveRegisteredPersonEndpoint
{
    public static async Task<IResult> Handle(
        ResolveRegisteredPersonRequest request,
        ResolveRegisteredPersonHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.Handle(request, cancellationToken);
            return Results.Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new ProblemDetails { Title = ex.Message });
        }
        catch (InvalidChangeOfAddressTransitionException ex)
        {
            return Results.Conflict(new ProblemDetails { Title = ex.Message });
        }
    }
}
