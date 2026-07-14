using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ClaimChangeOfAddressCase;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ClaimChangeOfAddressCase;

public static class ClaimChangeOfAddressCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimChangeOfAddressCaseHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(new ChangeOfAddressCaseId(id), cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidChangeOfAddressTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}