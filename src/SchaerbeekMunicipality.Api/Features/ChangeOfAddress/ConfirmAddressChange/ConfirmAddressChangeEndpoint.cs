using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ConfirmAddressChange;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ConfirmAddressChange;

public static class ConfirmAddressChangeEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ConfirmAddressChangeHandler handler,
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
