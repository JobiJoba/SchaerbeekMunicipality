using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.ClaimBirthDeclarationCase;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.ClaimBirthDeclarationCase;

public static class ClaimBirthDeclarationCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimBirthDeclarationCaseHandler handler,
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
