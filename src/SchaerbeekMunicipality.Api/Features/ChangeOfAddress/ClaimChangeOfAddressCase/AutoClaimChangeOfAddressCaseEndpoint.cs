using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ClaimChangeOfAddressCase;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;

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