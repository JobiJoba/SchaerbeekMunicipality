using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Validation;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordImmigrationDecision;

public static class RecordImmigrationDecisionEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RecordImmigrationDecisionRequest request,
        RecordImmigrationDecisionHandler handler,
        IValidator<RecordImmigrationDecisionRequest> validator,
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
