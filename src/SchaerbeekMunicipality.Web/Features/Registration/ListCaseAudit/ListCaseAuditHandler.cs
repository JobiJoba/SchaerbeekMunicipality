using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ListCaseAudit;

public sealed class ListCaseAuditHandler(ICaseAuditRepository auditRepository)
{
    public async Task<ListCaseAuditResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
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
