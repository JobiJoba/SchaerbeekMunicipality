using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ListChangeOfAddressCases;
namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.ListChangeOfAddressCases;

public static class ListChangeOfAddressCasesEndpoint
{
    public static async Task<IResult> Handle(
        ListChangeOfAddressCasesHandler handler,
        CancellationToken cancellationToken)
    {
        var cases = await handler.Handle(cancellationToken);
        return Results.Ok(cases);
    }
}
