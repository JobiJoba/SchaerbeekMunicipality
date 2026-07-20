using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ListDeathDeclarationCases;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.ListDeathDeclarationCases;

public static class ListDeathDeclarationCasesEndpoint
{
    public static async Task<IResult> Handle(
        ListDeathDeclarationCasesHandler handler,
        CancellationToken cancellationToken)
    {
        var cases = await handler.Handle(cancellationToken);
        return Results.Ok(cases);
    }
}
