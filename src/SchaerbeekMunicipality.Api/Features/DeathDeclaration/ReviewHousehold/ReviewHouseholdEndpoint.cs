using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ReviewHousehold;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.ReviewHousehold;

public static class ReviewHouseholdEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ReviewHouseholdHandler handler,
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
