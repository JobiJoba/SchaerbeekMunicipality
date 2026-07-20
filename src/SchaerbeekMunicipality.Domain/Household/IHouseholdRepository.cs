using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Household;

public interface IHouseholdRepository
{
    Task<Household?> GetByCaseIdAsync(RegistrationCaseId caseId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Household>> ListByMemberIdentityAsync(
        string givenName,
        string familyName,
        DateOnly birthDate,
        CancellationToken cancellationToken);

    Task AddAsync(Household household, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}