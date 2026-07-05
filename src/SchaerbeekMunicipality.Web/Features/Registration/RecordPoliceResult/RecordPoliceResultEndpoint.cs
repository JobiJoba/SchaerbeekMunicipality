using FluentValidation;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Validation;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordPoliceResult;

public static class RecordPoliceResultEndpoint
{
    public static async Task<IResult> Handle(
        Guid requestId,
        RecordPoliceResultRequest request,
        RecordPoliceResultHandler handler,
        IValidator<RecordPoliceResultRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationResults.ToProblemDetails(validation);
        }

        try
        {
            var result = await handler.Handle(
                PoliceVerificationRequestId.From(requestId),
                request,
                cancellationToken);

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
        catch (InvalidPoliceVerificationException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Police verification conflict");
        }
    }
}
