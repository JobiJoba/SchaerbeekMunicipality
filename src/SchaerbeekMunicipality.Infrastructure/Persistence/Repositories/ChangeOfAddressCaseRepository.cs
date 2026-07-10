using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class ChangeOfAddressCaseRepository(MunicipalDbContext dbContext) : IChangeOfAddressCaseRepository
{
    public async Task<IReadOnlyList<ChangeOfAddressCase>> ListAsync(CancellationToken cancellationToken)
    {
        var results = await dbContext.ChangeOfAddressCases
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return results
            .OrderByDescending(c => c.OpenedAt)
            .ToList();
    }

    public Task<ChangeOfAddressCase?> GetByIdAsync(ChangeOfAddressCaseId id, CancellationToken cancellationToken)
    {
        return dbContext.ChangeOfAddressCases
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(ChangeOfAddressCase changeOfAddressCase, CancellationToken cancellationToken)
    {
        await dbContext.ChangeOfAddressCases.AddAsync(changeOfAddressCase, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
