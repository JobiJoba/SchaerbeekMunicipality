using SchaerbeekMunicipality.Application.Features.PersonFile.ListPersonCases;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Api.Features.PersonFile.ListPersonCases;

public static class ListPersonCasesEndpoint
{
    public static async Task<IResult> Handle(
        Guid personId,
        ListPersonCasesHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new PersonId(personId), cancellationToken);
        return Results.Ok(result);
    }
}
