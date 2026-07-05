using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ResumeCase;

public static class ResumeCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        ResumeCaseHandler handler,
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
