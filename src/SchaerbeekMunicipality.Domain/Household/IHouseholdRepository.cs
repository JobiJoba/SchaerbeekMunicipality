namespace SchaerbeekMunicipality.Domain.Household;

public interface IHouseholdRepository
{
    Task<Household?> GetByCaseIdAsync(Registration.RegistrationCaseId caseId, CancellationToken cancellationToken);

    Task AddAsync(Household household, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
