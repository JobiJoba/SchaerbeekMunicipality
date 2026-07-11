using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.GetChangeOfAddressCase;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.GetChangeOfAddressCase;

public static class GetChangeOfAddressCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetChangeOfAddressCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var detail = await handler.Handle(new ChangeOfAddressCaseId(id), cancellationToken);
        return detail is null ? Results.NotFound() : Results.Ok(detail);
    }
}
