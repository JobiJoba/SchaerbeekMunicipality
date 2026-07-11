using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.Registration.ReleaseCaseLock;

public sealed record ReleaseCaseLockResponse(
    Guid CaseId,
    Guid? AssignedOfficerId,
    bool CanEdit);

public sealed class ReleaseCaseLockHandler(
    IRegistrationCaseRepository caseRepository,
    RegistrationCaseAuthorization authorization,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer)
{
    public async Task<ReleaseCaseLockResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanClaim(currentOfficer);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        registrationCase.ReleaseLock(officerId);

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.CaseLockReleased,
            null,
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ReleaseCaseLockResponse(
            registrationCase.Id.Value,
            registrationCase.AssignedOfficerId?.Value,
            false);
    }
}
