using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;

namespace SchaerbeekMunicipality.Api.Features.Registration.OpenRegistrationCase;

public static class OpenRegistrationCaseEndpoint
{
    public static async Task<IResult> Handle(
        OpenRegistrationCaseRequest request,
        OpenRegistrationCaseHandler handler,
        IValidator<OpenRegistrationCaseRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

        var result = await handler.Handle(request, cancellationToken);
        return Results.Created($"/api/registration/cases/{result.CaseId}", result);
    }
}