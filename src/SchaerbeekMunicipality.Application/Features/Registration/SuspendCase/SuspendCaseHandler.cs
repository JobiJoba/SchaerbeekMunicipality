using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.Registration.SuspendCase;

public sealed class SuspendCaseHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<SuspendCaseRequest> validator)
{
    public async Task<SuspendCaseResponse> Handle(
        RegistrationCaseId caseId,
        SuspendCaseRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
        {
            throw new UnauthorizedAccessException("Only population officers can suspend registration cases.");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(SuspendCase),
            cancellationToken);

        var officer = OfficerId.From(currentOfficer.OfficerId);
        registrationCase.Suspend(
            officer,
            request.Reason,
            request.Notes,
            timeProvider.GetUtcNow());

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.CaseSuspended,
            $"{request.Reason}: {request.Notes}",
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new SuspendCaseResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            request.Reason.ToString());
    }
}
