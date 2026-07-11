using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.Registration.RejectCase;

public sealed class RejectCaseHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<RejectCaseRequest> validator)
{
    public async Task<RejectCaseResponse> Handle(
        RegistrationCaseId caseId,
        RejectCaseRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
        {
            throw new UnauthorizedAccessException("Only population officers can reject registration cases.");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RejectCase),
            cancellationToken);

        var officer = OfficerId.From(currentOfficer.OfficerId);
        registrationCase.Reject(
            officer,
            request.Reason,
            request.Notes,
            timeProvider.GetUtcNow());

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.CaseRejected,
            $"{request.Reason}: {request.Notes}",
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RejectCaseResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            request.Reason.ToString());
    }
}
