using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class RegistrationCaseRepository(MunicipalDbContext dbContext) : IRegistrationCaseRepository
{
    public async Task<IReadOnlyList<RegistrationCase>> ListAsync(CancellationToken cancellationToken)
    {
        var cases = await dbContext.RegistrationCases
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return cases
            .OrderByDescending(c => c.OpenedAt)
            .ToList();
    }

    public Task<RegistrationCase?> GetByIdAsync(RegistrationCaseId id, CancellationToken cancellationToken)
    {
        return dbContext.RegistrationCases
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<RegistrationCase?> GetLatestRegisteredByPersonIdAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var cases = await dbContext.RegistrationCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId && c.Status == RegistrationCaseStatus.Registered)
            .ToListAsync(cancellationToken);

        return cases
            .OrderByDescending(c => c.ClosedAt ?? c.OpenedAt)
            .FirstOrDefault();
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