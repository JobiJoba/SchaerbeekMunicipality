using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ClaimDeathDeclarationCase;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.ClaimDeathDeclarationCase;

public static class AutoClaimDeathDeclarationCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimDeathDeclarationCaseHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.TryAutoClaimAsync(new DeathDeclarationCaseId(id), cancellationToken);
            return response is null ? Results.NoContent() : Results.Ok(response);
        }
        catch (InvalidDeathDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
