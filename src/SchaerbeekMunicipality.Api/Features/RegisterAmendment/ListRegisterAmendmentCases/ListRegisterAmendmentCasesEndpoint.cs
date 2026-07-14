using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ListRegisterAmendmentCases;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.ListRegisterAmendmentCases;

public static class ListRegisterAmendmentCasesEndpoint
{
    public static async Task<IResult> Handle(
        ListRegisterAmendmentCasesHandler handler,
        CancellationToken cancellationToken)
    {
        var cases = await handler.Handle(cancellationToken);
        return Results.Ok(cases);
    }
}
