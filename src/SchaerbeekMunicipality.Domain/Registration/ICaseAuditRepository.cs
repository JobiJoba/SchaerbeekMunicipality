namespace SchaerbeekMunicipality.Domain.Registration;

public interface ICaseAuditRepository
{
    Task<IReadOnlyList<CaseAuditEntry>> ListByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken);

    Task AddAsync(CaseAuditEntry entry, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}