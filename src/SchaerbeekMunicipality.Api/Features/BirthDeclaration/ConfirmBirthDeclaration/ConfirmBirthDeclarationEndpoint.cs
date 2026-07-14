using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ConfirmBirthDeclaration;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.ConfirmBirthDeclaration;

public static class ConfirmBirthDeclarationEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ConfirmBirthDeclarationHandler handler,
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
        catch (NationalRegisterConflictException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}