using SchaerbeekMunicipality.Application.Features.RegisterAmendment.SubmitRegisterAmendmentForReview;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.SubmitRegisterAmendmentForReview;

public static class SubmitRegisterAmendmentForReviewEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        SubmitRegisterAmendmentForReviewHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(RegisterAmendmentCaseId.From(id), cancellationToken);
            return Results.Ok(result);
        }
        catch (InvalidRegisterAmendmentTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
