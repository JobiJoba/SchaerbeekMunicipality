using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;

public sealed class GetRegistrationCaseHandler(
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository,
    IResidencePermitRepository permitRepository)
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
        var permit = await permitRepository.GetByCaseIdAsync(caseId, cancellationToken);

        return Map(registrationCase, person, documents, permit);
    }

    private static RegistrationCaseDetailDto Map(
        RegistrationCase registrationCase,
        Person? person,
        IReadOnlyList<AdministrativeDocument> documents,
        ResidencePermit? permit)
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
            registrationCase.ResidenceCategory,
            permit is null
                ? null
                : new ResidencePermitDto(
                    permit.Id.Value,
                    permit.PermitType,
                    permit.CardNumber,
                    permit.ValidFrom,
                    permit.ValidUntil,
                    permit.IssuingAuthority,
                    permit.RecordedAt),
            registrationCase.ImmigrationDecision is null
                ? null
                : new ImmigrationDecisionDto(
                    registrationCase.ImmigrationDecision.ReferenceNumber,
                    registrationCase.ImmigrationDecision.DecisionDate),
            documents
                .Select(d => new DocumentDto(
                    d.Id.Value,
                    d.DocumentType,
                    d.FileName,
                    d.UploadedAt))
                .ToList());
    }
}
