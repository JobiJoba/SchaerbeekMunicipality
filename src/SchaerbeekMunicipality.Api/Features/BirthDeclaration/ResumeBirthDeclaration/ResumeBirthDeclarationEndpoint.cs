using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ResumeBirthDeclaration;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.ResumeBirthDeclaration;

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