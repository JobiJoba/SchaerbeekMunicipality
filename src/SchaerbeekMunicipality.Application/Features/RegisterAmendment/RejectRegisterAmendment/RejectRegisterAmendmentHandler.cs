using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.RejectRegisterAmendment;

public sealed class RejectRegisterAmendmentValidator : AbstractValidator<RejectRegisterAmendmentRequest>
{
    public RejectRegisterAmendmentValidator()
    {
        RuleFor(r => r.Reason).IsInEnum();
    }
}

public sealed class RejectRegisterAmendmentHandler(
    RegisterAmendmentCaseGuard caseGuard,
    IRegisterAmendmentCaseRepository caseRepository,
    RegisterAmendmentCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<RejectRegisterAmendmentRequest> validator)
{
    public async Task<RejectRegisterAmendmentResponse> Handle(
        RegisterAmendmentCaseId caseId,
        RejectRegisterAmendmentRequest request,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanApprove(currentOfficer);
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var amendmentCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RejectRegisterAmendment),
            cancellationToken);

        amendmentCase.Reject(
            OfficerId.From(currentOfficer.OfficerId),
            request.Reason,
            request.Notes,
            timeProvider.GetUtcNow());

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RejectRegisterAmendmentResponse(
            amendmentCase.Id.Value,
            amendmentCase.Status.ToString(),
            request.Reason.ToString());
    }
}
