using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Police;

public interface IPoliceVerificationRepository
{
    Task<PoliceVerificationRequest?> GetByIdAsync(
        PoliceVerificationRequestId id,
        CancellationToken cancellationToken);

    Task<PoliceVerificationRequest?> GetPendingByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken);

    Task<PoliceVerificationRequest?> GetPendingByChangeOfAddressCaseIdAsync(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PoliceVerificationRequest>> ListPendingAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PoliceVerificationRequest>> ListAllAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PoliceVerificationRequest>> ListByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken);

    Task<int> CountPendingAsync(CancellationToken cancellationToken);

    Task<int> GetMaxAttemptNumberAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken);

    Task<int> GetMaxAttemptNumberForChangeOfAddressAsync(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken);

    Task AddAsync(PoliceVerificationRequest request, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}