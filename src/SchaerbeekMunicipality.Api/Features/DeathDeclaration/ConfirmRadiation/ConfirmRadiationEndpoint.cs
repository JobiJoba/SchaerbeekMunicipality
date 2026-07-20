using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ConfirmRadiation;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.ConfirmRadiation;

public static class ConfirmRadiationEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ConfirmRadiationHandler handler,
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
