using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class RegistrationCaseRepository(MunicipalDbContext dbContext) : IRegistrationCaseRepository
{
    public async Task<IReadOnlyList<RegistrationCase>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.RegistrationCases
            .AsNoTracking()
            .OrderByDescending(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<RegistrationCase?> GetByIdAsync(RegistrationCaseId id, CancellationToken cancellationToken)
    {
        return dbContext.RegistrationCases
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(RegistrationCase registrationCase, CancellationToken cancellationToken)
    {
        await dbContext.RegistrationCases.AddAsync(registrationCase, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
