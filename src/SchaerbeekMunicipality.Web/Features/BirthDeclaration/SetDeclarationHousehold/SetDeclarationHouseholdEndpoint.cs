using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Web.Features.BirthDeclaration.SetDeclarationHousehold;
using SchaerbeekMunicipality.Web.Validation;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.SetDeclarationHousehold;

public static class SetDeclarationHouseholdEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        SetDeclarationHouseholdRequest request,
        SetDeclarationHouseholdHandler handler,
        IValidator<SetDeclarationHouseholdRequest> validator,
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
