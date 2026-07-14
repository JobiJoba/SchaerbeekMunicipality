using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Certificates;

public interface ICertificateRequestRepository
{
    Task AddAsync(CertificateRequest certificateRequest, CancellationToken cancellationToken);

    Task<IReadOnlyList<CertificateRequest>> ListByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CertificateRequest>> ListByPersonIdAsync(
        PersonId personId,
        CancellationToken cancellationToken);

    Task<int> CountByTypeAsync(CertificateType certificateType, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}