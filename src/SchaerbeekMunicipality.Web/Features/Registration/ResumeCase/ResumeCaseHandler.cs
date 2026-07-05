using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.ResumeCase;

public sealed class ResumeCaseHandler(
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

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

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
