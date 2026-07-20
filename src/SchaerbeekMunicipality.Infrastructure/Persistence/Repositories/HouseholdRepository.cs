using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class HouseholdRepository(MunicipalDbContext dbContext) : IHouseholdRepository
{
    public Task<Household?> GetByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        return dbContext.Households
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.RegistrationCaseId == caseId, cancellationToken);
    }

    public async Task<IReadOnlyList<Household>> ListByMemberIdentityAsync(
        string givenName,
        string familyName,
        DateOnly birthDate,
        CancellationToken cancellationToken)
    {
        var normalizedGiven = givenName.Trim();
        var normalizedFamily = familyName.Trim();

        var households = await dbContext.Households
            .Include(h => h.Members)
            .ToListAsync(cancellationToken);

        return households
            .Where(h => h.Members.Any(m =>
                string.Equals(m.GivenName, normalizedGiven, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(m.FamilyName, normalizedFamily, StringComparison.OrdinalIgnoreCase) &&
                m.BirthDate == birthDate))
            .ToList();
    }

    public async Task AddAsync(Household household, CancellationToken cancellationToken)
    {
        await dbContext.Households.AddAsync(household, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}