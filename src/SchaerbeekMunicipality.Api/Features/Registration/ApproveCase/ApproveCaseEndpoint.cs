using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;

namespace SchaerbeekMunicipality.Api.Features.Registration.ApproveCase;

public static class ApproveCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ApproveCaseRequest request,
        ApproveCaseHandler handler,
        IValidator<ApproveCaseRequest> validator,
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
