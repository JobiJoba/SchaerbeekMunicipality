using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.Registration;

public interface IRegistrationCaseRepository
{
    Task<IReadOnlyList<RegistrationCase>> ListAsync(CancellationToken cancellationToken);

    Task<RegistrationCase?> GetByIdAsync(RegistrationCaseId id, CancellationToken cancellationToken);

    Task<RegistrationCase?> GetLatestRegisteredByPersonIdAsync(
        PersonId personId,
        CancellationToken cancellationToken);

    Task AddAsync(RegistrationCase registrationCase, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}