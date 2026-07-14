using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.SubmitRegisterAmendmentForReview;

public sealed record SubmitRegisterAmendmentForReviewResponse(
    Guid CaseId,
    string Status,
    DateTimeOffset SubmittedAt);

public sealed class SubmitRegisterAmendmentForReviewHandler(
    RegisterAmendmentCaseGuard caseGuard,
    IRegisterAmendmentCaseRepository caseRepository,
    TimeProvider timeProvider)
{
    public async Task<SubmitRegisterAmendmentForReviewResponse> Handle(
        RegisterAmendmentCaseId caseId,
        CancellationToken cancellationToken)
    {
        var amendmentCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(SubmitRegisterAmendmentForReview),
            cancellationToken);

        var submittedAt = timeProvider.GetUtcNow();
        amendmentCase.SubmitForReview(submittedAt);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new SubmitRegisterAmendmentForReviewResponse(
            amendmentCase.Id.Value,
            amendmentCase.Status.ToString(),
            submittedAt);
    }
}
