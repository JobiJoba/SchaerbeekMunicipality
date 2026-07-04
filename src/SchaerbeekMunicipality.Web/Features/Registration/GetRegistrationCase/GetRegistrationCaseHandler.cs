using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;

public sealed class GetRegistrationCaseHandler(
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository)
{
    public async Task<RegistrationCaseDetailDto?> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken);
        if (registrationCase is null)
        {
            return null;
        }

        Person? person = null;
        if (registrationCase.PersonId is { } personId)
        {
            person = await personRepository.GetByIdAsync(personId, cancellationToken);
        }

        var documents = await documentRepository.ListByCaseIdAsync(caseId, cancellationToken);

        return Map(registrationCase, person, documents);
    }

    private static RegistrationCaseDetailDto Map(
        RegistrationCase registrationCase,
        Person? person,
        IReadOnlyList<AdministrativeDocument> documents)
    {
        var checklist = registrationCase.Checklist;

        return new RegistrationCaseDetailDto(
            registrationCase.Id.Value,
            registrationCase.Status,
            registrationCase.VisitReason,
            registrationCase.AssignedOfficerId.Value,
            registrationCase.OpenedAt,
            new RegistrationCaseChecklistDto(
                checklist.IdentityEstablished,
                checklist.LegalResidenceEstablished,
                checklist.AddressDeclared,
                checklist.AddressConfirmed,
                checklist.RegisterDeterminable),
            person is null
                ? null
                : new PersonDto(
                    person.Id.Value,
                    person.GivenName,
                    person.FamilyName,
                    person.BirthDate,
                    person.Nationality),
            documents
                .Select(d => new DocumentDto(
                    d.Id.Value,
                    d.DocumentType,
                    d.FileName,
                    d.UploadedAt))
                .ToList());
    }
}
