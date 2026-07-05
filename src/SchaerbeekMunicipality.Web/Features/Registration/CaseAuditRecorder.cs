using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration;

public sealed class CaseAuditRecorder(
    ICaseAuditRepository auditRepository,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task RecordAsync(
        RegistrationCaseId caseId,
        CaseAuditAction action,
        string? details,
        CancellationToken cancellationToken)
    {
        var entry = CaseAuditEntry.Create(
            caseId,
            action,
            OfficerId.From(currentOfficer.OfficerId),
            timeProvider.GetUtcNow(),
            details);

        await auditRepository.AddAsync(entry, cancellationToken);
    }
}
