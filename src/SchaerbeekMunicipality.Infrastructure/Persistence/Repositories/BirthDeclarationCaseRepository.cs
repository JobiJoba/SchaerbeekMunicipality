using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class BirthDeclarationCaseRepository(MunicipalDbContext dbContext) : IBirthDeclarationCaseRepository
{
    public async Task<IReadOnlyList<BirthDeclarationCase>> ListAsync(CancellationToken cancellationToken)
    {
        var cases = await dbContext.BirthDeclarationCases
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return cases
            .OrderByDescending(c => c.OpenedAt)
            .ToList();
    }

    public Task<BirthDeclarationCase?> GetByIdAsync(BirthDeclarationCaseId id, CancellationToken cancellationToken)
    {
        return dbContext.BirthDeclarationCases
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(BirthDeclarationCase birthDeclarationCase, CancellationToken cancellationToken)
    {
        await dbContext.BirthDeclarationCases.AddAsync(birthDeclarationCase, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}