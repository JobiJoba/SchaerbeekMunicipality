using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ResumeDeathDeclaration;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.ResumeDeathDeclaration;

public static class ResumeDeathDeclarationEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ResumeDeathDeclarationHandler handler,
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
