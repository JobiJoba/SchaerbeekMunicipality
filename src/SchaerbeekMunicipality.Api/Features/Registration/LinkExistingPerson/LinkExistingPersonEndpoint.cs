using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.Registration.LinkExistingPerson;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Api.Features.Registration.LinkExistingPerson;

public static class LinkExistingPersonEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        LinkExistingPersonRequest request,
        LinkExistingPersonHandler handler,
        IValidator<LinkExistingPersonRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

        try
        {
            var result = await handler.Handle(new RegistrationCaseId(id), request, cancellationToken);
            return Results.Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { detail = ex.Message });
        }
        catch (InvalidRegistrationTransitionException ex)
        {
            return Results.Problem(
                ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Invalid registration transition");
        }
        catch (NationalRegisterConflictException ex)
        {
            return Results.Problem(
                ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "National Register conflict");
        }
    }
}