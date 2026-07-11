using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;

namespace SchaerbeekMunicipality.Api.Features.Registration.RequestPoliceVerification;

public static class RequestPoliceVerificationEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        RequestPoliceVerificationHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(new RegistrationCaseId(id), cancellationToken);
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
