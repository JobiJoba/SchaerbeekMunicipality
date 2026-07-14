using SchaerbeekMunicipality.Application.Features.Registration.GetCaseReviewChecklist;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Api.Features.Registration.GetCaseReviewChecklist;

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