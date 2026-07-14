using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Household;

public interface IHouseholdRepository
{
    Task<Household?> GetByCaseIdAsync(RegistrationCaseId caseId, CancellationToken cancellationToken);

    Task AddAsync(Household household, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}