using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ClaimChangeOfAddressCase;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ClaimChangeOfAddressCase;

public static class AutoClaimChangeOfAddressCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimChangeOfAddressCaseHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.TryAutoClaimAsync(new ChangeOfAddressCaseId(id), cancellationToken);
            return response is null ? Results.NoContent() : Results.Ok(response);
        }
        catch (InvalidChangeOfAddressTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
