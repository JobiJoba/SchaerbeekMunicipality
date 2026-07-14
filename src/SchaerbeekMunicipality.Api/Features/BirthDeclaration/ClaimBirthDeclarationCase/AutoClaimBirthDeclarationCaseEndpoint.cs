using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ClaimBirthDeclarationCase;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.ClaimBirthDeclarationCase;

public static class AutoClaimBirthDeclarationCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimBirthDeclarationCaseHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.TryAutoClaimAsync(new BirthDeclarationCaseId(id), cancellationToken);
            return response is null ? Results.NoContent() : Results.Ok(response);
        }
        catch (InvalidBirthDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}