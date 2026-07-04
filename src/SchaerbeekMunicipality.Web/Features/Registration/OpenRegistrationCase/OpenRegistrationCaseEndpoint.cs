using FluentValidation;
using SchaerbeekMunicipality.Web.Validation;

namespace SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;

public static class OpenRegistrationCaseEndpoint
{
    public static async Task<IResult> Handle(
        OpenRegistrationCaseRequest request,
        OpenRegistrationCaseHandler handler,
        IValidator<OpenRegistrationCaseRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationResults.ToProblemDetails(validation);
        }

        var result = await handler.Handle(request, cancellationToken);
        return Results.Created($"/api/registration/cases/{result.CaseId}", result);
    }
}
