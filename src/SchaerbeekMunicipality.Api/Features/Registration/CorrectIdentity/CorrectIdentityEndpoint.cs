using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.Registration.CorrectIdentity;

namespace SchaerbeekMunicipality.Api.Features.Registration.CorrectIdentity;

public static class CorrectIdentityEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RecordIdentityRequest request,
        CorrectIdentityHandler handler,
        IValidator<RecordIdentityRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationResults.ToProblemDetails(validation);
        }

        try
        {
            var result = await handler.Handle(new RegistrationCaseId(id), request, cancellationToken);
            return Results.Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidRegistrationTransitionException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Invalid registration transition");
        }
    }
}
