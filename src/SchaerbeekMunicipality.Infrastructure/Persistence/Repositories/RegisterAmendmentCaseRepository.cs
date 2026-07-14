using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class RegisterAmendmentCaseRepository(MunicipalDbContext dbContext) : IRegisterAmendmentCaseRepository
{
    public async Task<IReadOnlyList<RegisterAmendmentCase>> ListAsync(CancellationToken cancellationToken)
    {
        var results = await dbContext.RegisterAmendmentCases
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return results
            .OrderByDescending(c => c.OpenedAt)
            .ToList();
    }

    public Task<RegisterAmendmentCase?> GetByIdAsync(RegisterAmendmentCaseId id, CancellationToken cancellationToken)
    {
        return dbContext.RegisterAmendmentCases
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<bool> HasOpenCaseForPersonAsync(PersonId personId, CancellationToken cancellationToken)
    {
        return dbContext.RegisterAmendmentCases
            .AsNoTracking()
            .AnyAsync(
                c => c.PersonId == personId &&
                     c.Status != RegisterAmendmentCaseStatus.Applied &&
                     c.Status != RegisterAmendmentCaseStatus.Rejected,
                cancellationToken);
    }

    public async Task AddAsync(RegisterAmendmentCase registerAmendmentCase, CancellationToken cancellationToken)
    {
        await dbContext.RegisterAmendmentCases.AddAsync(registerAmendmentCase, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
