using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.RejectBirthDeclaration;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.RejectBirthDeclaration;

public static class RejectBirthDeclarationEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RejectBirthDeclarationRequest request,
        RejectBirthDeclarationHandler handler,
        IValidator<RejectBirthDeclarationRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

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