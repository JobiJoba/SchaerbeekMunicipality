using Microsoft.AspNetCore.Mvc;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration.ClaimRegistrationCase;

namespace SchaerbeekMunicipality.Api.Features.Registration.ClaimRegistrationCase;

public static class AutoClaimRegistrationCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ClaimRegistrationCaseHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.TryAutoClaimAsync(new RegistrationCaseId(id), cancellationToken);
            return response is null ? Results.NoContent() : Results.Ok(response);
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
