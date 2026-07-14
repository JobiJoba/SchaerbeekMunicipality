using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class NationalRegisterRepository(MunicipalDbContext dbContext) : INationalRegisterRepository
{
    public Task<NationalRegisterPerson?> GetByIdAsync(
        NationalRegisterPersonId id,
        CancellationToken cancellationToken)
    {
        return dbContext.NationalRegisterPersons
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<NationalRegisterMatch>> SearchAsync(
        NationalRegisterSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var candidates = await dbContext.NationalRegisterPersons
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return NationalRegisterSearchScorer.ScoreMatches(criteria, candidates);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}