using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Documents;

public interface IAdministrativeDocumentRepository
{
    Task<IReadOnlyList<AdministrativeDocument>> ListByRegistrationCaseIdAsync(
        RegistrationCaseId registrationCaseId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AdministrativeDocument>> ListByBirthDeclarationCaseIdAsync(
        BirthDeclarationCaseId birthDeclarationCaseId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AdministrativeDocument>> ListByChangeOfAddressCaseIdAsync(
        ChangeOfAddressCaseId changeOfAddressCaseId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AdministrativeDocument>> ListByDocumentRequestCaseIdAsync(
        DocumentRequestCaseId documentRequestCaseId,
        CancellationToken cancellationToken);

    Task<AdministrativeDocument?> GetByIdAsync(
        AdministrativeDocumentId id,
        CancellationToken cancellationToken);

    Task AddAsync(AdministrativeDocument document, CancellationToken cancellationToken);

    void Remove(AdministrativeDocument document);
}
