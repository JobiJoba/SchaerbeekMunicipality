using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Immigration;

public interface IResidencePermitRepository
{
    Task<ResidencePermit?> GetByCaseIdAsync(RegistrationCaseId caseId, CancellationToken cancellationToken);

    Task AddAsync(ResidencePermit permit, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
