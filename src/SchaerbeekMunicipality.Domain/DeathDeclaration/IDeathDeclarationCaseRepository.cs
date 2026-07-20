using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.DeathDeclaration;

public interface IDeathDeclarationCaseRepository
{
    Task<IReadOnlyList<DeathDeclarationCase>> ListAsync(CancellationToken cancellationToken);

    Task<DeathDeclarationCase?> GetByIdAsync(DeathDeclarationCaseId id, CancellationToken cancellationToken);

    Task<DeathDeclarationCase?> GetActiveByPersonIdAsync(PersonId personId, CancellationToken cancellationToken);

    Task AddAsync(DeathDeclarationCase deathDeclarationCase, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
