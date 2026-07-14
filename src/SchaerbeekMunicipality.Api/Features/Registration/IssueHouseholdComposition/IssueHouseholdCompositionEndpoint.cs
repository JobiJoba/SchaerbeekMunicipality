using SchaerbeekMunicipality.Application.Features.Registration.IssueHouseholdComposition;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Api.Features.Registration.IssueHouseholdComposition;

public static class IssueHouseholdCompositionEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IssueHouseholdCompositionHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(new RegistrationCaseId(id), cancellationToken);
            return Results.Content(result.HtmlContent, "text/html; charset=utf-8");
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidRegistrationTransitionException ex)
        {
            return Results.Problem(
                ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Cannot issue certificate");
        }
    }
}