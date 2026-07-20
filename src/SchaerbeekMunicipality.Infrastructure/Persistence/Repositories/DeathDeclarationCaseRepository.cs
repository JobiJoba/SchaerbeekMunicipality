using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class DeathDeclarationCaseRepository(MunicipalDbContext dbContext) : IDeathDeclarationCaseRepository
{
    public async Task<IReadOnlyList<DeathDeclarationCase>> ListAsync(CancellationToken cancellationToken)
    {
        var cases = await dbContext.DeathDeclarationCases
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return cases
            .OrderByDescending(c => c.OpenedAt)
            .ToList();
    }

    public Task<DeathDeclarationCase?> GetByIdAsync(DeathDeclarationCaseId id, CancellationToken cancellationToken)
    {
        return dbContext.DeathDeclarationCases
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<DeathDeclarationCase?> GetActiveByPersonIdAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var cases = await dbContext.DeathDeclarationCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        return cases.FirstOrDefault(c =>
            c.Status is DeathDeclarationCaseStatus.Intake
                or DeathDeclarationCaseStatus.UnderReview
                or DeathDeclarationCaseStatus.Suspended);
    }

    public async Task AddAsync(DeathDeclarationCase deathDeclarationCase, CancellationToken cancellationToken)
    {
        await dbContext.DeathDeclarationCases.AddAsync(deathDeclarationCase, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
