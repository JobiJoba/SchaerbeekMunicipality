using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.UpdateHouseholdForMove;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.UpdateHouseholdForMove;

public static class UnlinkHouseholdMemberEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        Guid personId,
        UnlinkHouseholdMemberHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(
                new ChangeOfAddressCaseId(id),
                new PersonId(personId),
                cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidChangeOfAddressTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}