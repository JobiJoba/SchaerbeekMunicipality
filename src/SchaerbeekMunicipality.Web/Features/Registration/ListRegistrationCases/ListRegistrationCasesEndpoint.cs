namespace SchaerbeekMunicipality.Web.Features.Registration.ListRegistrationCases;

public static class ListRegistrationCasesEndpoint
{
    public static async Task<IResult> Handle(
        ListRegistrationCasesHandler handler,
        CancellationToken cancellationToken)
    {
        var cases = await handler.Handle(cancellationToken);
        return Results.Ok(cases);
    }
}
