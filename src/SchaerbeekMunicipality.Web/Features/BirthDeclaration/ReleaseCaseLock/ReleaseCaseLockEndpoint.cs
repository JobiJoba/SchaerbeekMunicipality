using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.ReleaseCaseLock;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.ReleaseCaseLock;

public static class ReleaseCaseLockEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ReleaseCaseLockHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(new BirthDeclarationCaseId(id), cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidBirthDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
