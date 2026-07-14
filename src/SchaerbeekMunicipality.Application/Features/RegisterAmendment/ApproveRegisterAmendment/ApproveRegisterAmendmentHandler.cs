using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.ApproveRegisterAmendment;

public sealed class ApproveRegisterAmendmentHandler(
    RegisterAmendmentCaseGuard caseGuard,
    IRegisterAmendmentCaseRepository caseRepository,
    RegisterAmendmentCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ApproveRegisterAmendmentResponse> Handle(
        RegisterAmendmentCaseId caseId,
        ApproveRegisterAmendmentRequest request,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanApprove(currentOfficer);

        var amendmentCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ApproveRegisterAmendment),
            cancellationToken);

        var approvedAt = timeProvider.GetUtcNow();
        amendmentCase.Approve(OfficerId.From(currentOfficer.OfficerId), request.Notes, approvedAt);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ApproveRegisterAmendmentResponse(
            amendmentCase.Id.Value,
            amendmentCase.Status.ToString(),
            approvedAt);
    }
}
