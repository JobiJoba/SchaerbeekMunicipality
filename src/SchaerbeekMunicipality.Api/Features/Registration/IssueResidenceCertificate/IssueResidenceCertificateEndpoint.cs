using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration.IssueResidenceCertificate;

namespace SchaerbeekMunicipality.Api.Features.Registration.IssueResidenceCertificate;

public static class IssueResidenceCertificateEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IssueResidenceCertificateHandler handler,
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
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Cannot issue certificate");
        }
    }
}
