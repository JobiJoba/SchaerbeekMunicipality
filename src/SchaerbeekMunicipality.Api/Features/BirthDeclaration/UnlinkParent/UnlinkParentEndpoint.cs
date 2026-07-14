using SchaerbeekMunicipality.Application.Features.BirthDeclaration.UnlinkParent;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.UnlinkParent;

public static class UnlinkParentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        Guid personId,
        UnlinkParentHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(
                new BirthDeclarationCaseId(id),
                new PersonId(personId),
                cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidBirthDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}