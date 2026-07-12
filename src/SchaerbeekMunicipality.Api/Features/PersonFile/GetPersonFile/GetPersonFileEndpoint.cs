using SchaerbeekMunicipality.Application.Features.PersonFile.GetPersonFile;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Api.Features.PersonFile.GetPersonFile;

public static class GetPersonFileEndpoint
{
    public static async Task<IResult> HandleById(
        Guid personId,
        GetPersonFileHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new PersonId(personId), cancellationToken);
        return Results.Ok(result);
    }

    public static async Task<IResult> HandleByNationalRegisterNumber(
        string nrNumber,
        GetPersonFileHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleByNationalRegisterNumber(nrNumber, cancellationToken);
        return Results.Ok(result);
    }
}
