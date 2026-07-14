using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.RegisterAmendment;

public interface IRegisterAmendmentCaseRepository
{
    Task<IReadOnlyList<RegisterAmendmentCase>> ListAsync(CancellationToken cancellationToken);

    Task<RegisterAmendmentCase?> GetByIdAsync(RegisterAmendmentCaseId id, CancellationToken cancellationToken);

    Task<bool> HasOpenCaseForPersonAsync(PersonId personId, CancellationToken cancellationToken);

    Task AddAsync(RegisterAmendmentCase registerAmendmentCase, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
