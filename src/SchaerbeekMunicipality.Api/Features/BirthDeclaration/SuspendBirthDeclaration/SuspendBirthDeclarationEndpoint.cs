using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.SuspendBirthDeclaration;
using SchaerbeekMunicipality.Api.Validation;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.SuspendBirthDeclaration;

public static class SuspendBirthDeclarationEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        SuspendBirthDeclarationRequest request,
        SuspendBirthDeclarationHandler handler,
        IValidator<SuspendBirthDeclarationRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationResults.ToProblemDetails(validation);
        }

        try
        {
            var result = await handler.Handle(new BirthDeclarationCaseId(id), request, cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidBirthDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
