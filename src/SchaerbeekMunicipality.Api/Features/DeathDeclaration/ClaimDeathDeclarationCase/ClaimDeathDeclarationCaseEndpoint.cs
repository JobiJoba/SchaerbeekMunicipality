using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ClaimDeathDeclarationCase;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.ClaimDeathDeclarationCase;

public static class ClaimDeathDeclarationCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimDeathDeclarationCaseHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(new DeathDeclarationCaseId(id), cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidDeathDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
