using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ClaimBirthDeclarationCase;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.ClaimBirthDeclarationCase;

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