using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;

namespace SchaerbeekMunicipality.Api.Features.Registration.DeclareAddress;

public static class DeclareAddressEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        DeclareAddressRequest request,
        DeclareAddressHandler handler,
        IValidator<DeclareAddressRequest> validator,
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
        catch (ArgumentException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid address");
        }
    }
}
