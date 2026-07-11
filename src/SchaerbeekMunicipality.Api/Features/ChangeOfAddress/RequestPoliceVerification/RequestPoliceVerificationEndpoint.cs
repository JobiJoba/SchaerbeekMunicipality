using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.RequestPoliceVerification;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.RequestPoliceVerification;

public static class RequestPoliceVerificationEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RequestPoliceVerificationHandler handler,
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
        catch (InvalidPoliceVerificationException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
