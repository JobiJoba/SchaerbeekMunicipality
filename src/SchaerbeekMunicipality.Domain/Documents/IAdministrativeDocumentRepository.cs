using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Documents;

public interface IAdministrativeDocumentRepository
{
    Task<IReadOnlyList<AdministrativeDocument>> ListByCaseIdAsync(
        RegistrationCaseId registrationCaseId,
        CancellationToken cancellationToken);

    Task AddAsync(AdministrativeDocument document, CancellationToken cancellationToken);
}
