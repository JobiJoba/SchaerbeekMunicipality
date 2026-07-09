using SchaerbeekMunicipality.Web.Features.BirthDeclaration.OpenBirthDeclarationCase;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.OpenBirthDeclarationCase;

public static class OpenBirthDeclarationCaseEndpoint
{
    public static async Task<IResult> Handle(
        OpenBirthDeclarationCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(cancellationToken);
        return Results.Created($"/api/birth-declarations/cases/{result.CaseId}", result);
    }
}
