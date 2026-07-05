namespace SchaerbeekMunicipality.Domain.Registration;

public sealed class CaseAuditEntry
{
    private CaseAuditEntry()
    {
    }

    public CaseAuditEntryId Id { get; private set; }

    public RegistrationCaseId CaseId { get; private set; }

    public CaseAuditAction Action { get; private set; }

    public OfficerId OfficerId { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    public string? Details { get; private set; }

    public static CaseAuditEntry Create(
        RegistrationCaseId caseId,
        CaseAuditAction action,
        OfficerId officerId,
        DateTimeOffset occurredAt,
        string? details = null)
    {
        return new CaseAuditEntry
        {
            Id = CaseAuditEntryId.New(),
            CaseId = caseId,
            Action = action,
            OfficerId = officerId,
            OccurredAt = occurredAt,
            Details = details?.Trim(),
        };
    }
}
