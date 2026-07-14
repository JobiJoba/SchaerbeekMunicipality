using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.ListCaseAudit;

public sealed class ListCaseAuditHandler(
    RegistrationCaseGuard caseGuard,
    RegistrationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    ICaseAuditRepository auditRepository)
{
    public async Task<ListCaseAuditResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);
        _ = await caseGuard.GetForViewAsync(caseId, cancellationToken);

        var entries = await auditRepository.ListByCaseIdAsync(caseId, cancellationToken);

        return new ListCaseAuditResponse(
            caseId.Value,
            entries.Select(e => new CaseAuditEntryDto(
                e.Id.Value,
                e.Action.ToString(),
                e.OfficerId.Value,
                e.OccurredAt,
                e.Details)).ToList());
    }
}