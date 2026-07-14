using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ListBirthDeclarationCases;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.ListBirthDeclarationCases;

public static class ListBirthDeclarationCasesEndpoint
{
    public static async Task<IResult> Handle(
        ListBirthDeclarationCasesHandler handler,
        CancellationToken cancellationToken)
    {
        var cases = await handler.Handle(cancellationToken);
        return Results.Ok(cases);
    }
}