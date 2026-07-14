using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.Registration.SuspendCase;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Api.Features.Registration.SuspendCase;

public static class SuspendCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        SuspendCaseRequest request,
        SuspendCaseHandler handler,
        IValidator<SuspendCaseRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

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
                ex.Message,
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden");
        }
        catch (InvalidRegistrationTransitionException ex)
        {
            return Results.Problem(
                ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Invalid registration transition");
        }
    }
}