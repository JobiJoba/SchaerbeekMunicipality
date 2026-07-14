using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ReleaseCaseLock;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ReleaseCaseLock;

public static class ReleaseCaseLockEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ReleaseCaseLockHandler handler,
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