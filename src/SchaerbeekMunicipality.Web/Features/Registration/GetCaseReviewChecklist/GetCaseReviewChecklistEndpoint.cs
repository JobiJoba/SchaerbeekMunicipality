using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetCaseReviewChecklist;

public static class GetCaseReviewChecklistEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetCaseReviewChecklistHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new RegistrationCaseId(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}
