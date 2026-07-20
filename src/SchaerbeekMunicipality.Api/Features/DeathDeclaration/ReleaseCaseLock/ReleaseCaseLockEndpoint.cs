using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ReleaseCaseLock;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.ReleaseCaseLock;

public static class ReleaseCaseLockEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ReleaseCaseLockHandler handler,
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
