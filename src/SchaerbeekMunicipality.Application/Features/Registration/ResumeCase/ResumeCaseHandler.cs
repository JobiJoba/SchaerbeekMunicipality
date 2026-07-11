using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.Registration.ResumeCase;

public sealed class ResumeCaseHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer)
{
    public async Task<ResumeCaseResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
        {
            throw new UnauthorizedAccessException("Only population officers can resume suspended cases.");
        }

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ResumeCase),
            cancellationToken);

        registrationCase.ResumeFromSuspension();

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.CaseResumed,
            null,
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ResumeCaseResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString());
    }
}
