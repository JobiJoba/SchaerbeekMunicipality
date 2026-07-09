using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.ResumeBirthDeclaration;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.ResumeBirthDeclaration;

public static class ResumeBirthDeclarationEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ResumeBirthDeclarationHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(new BirthDeclarationCaseId(id), cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidBirthDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
