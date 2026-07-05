using Microsoft.AspNetCore.Mvc;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ClaimRegistrationCase;

public static class ClaimRegistrationCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimRegistrationCaseHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.Handle(new RegistrationCaseId(id), cancellationToken);
            return Results.Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidRegistrationTransitionException ex)
        {
            return Results.Conflict(new ProblemDetails { Title = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Problem(statusCode: StatusCodes.Status403Forbidden, title: "Forbidden");
        }
    }
}
