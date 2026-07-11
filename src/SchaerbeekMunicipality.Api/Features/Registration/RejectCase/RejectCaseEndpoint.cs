using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.Registration.RejectCase;

namespace SchaerbeekMunicipality.Api.Features.Registration.RejectCase;

public static class RejectCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RejectCaseRequest request,
        RejectCaseHandler handler,
        IValidator<RejectCaseRequest> validator,
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
        catch (UnauthorizedAccessException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden");
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
