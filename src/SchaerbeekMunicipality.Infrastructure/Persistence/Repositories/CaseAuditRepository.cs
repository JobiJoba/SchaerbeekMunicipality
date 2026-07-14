using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class CaseAuditRepository(MunicipalDbContext dbContext) : ICaseAuditRepository
{
    public async Task<IReadOnlyList<CaseAuditEntry>> ListByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var entries = await dbContext.CaseAuditEntries
            .AsNoTracking()
            .Where(e => e.CaseId == caseId)
            .ToListAsync(cancellationToken);

        return entries
            .OrderByDescending(e => e.OccurredAt)
            .ToList();
    }

    public async Task AddAsync(CaseAuditEntry entry, CancellationToken cancellationToken)
    {
        await dbContext.CaseAuditEntries.AddAsync(entry, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}