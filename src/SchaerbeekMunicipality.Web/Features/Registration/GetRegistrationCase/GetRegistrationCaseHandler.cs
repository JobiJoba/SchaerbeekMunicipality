using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;

public sealed class GetRegistrationCaseHandler(
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository,
    IResidencePermitRepository permitRepository,
    IHouseholdRepository householdRepository)
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
        var household = await householdRepository.GetByCaseIdAsync(caseId, cancellationToken);

        return Map(registrationCase, person, documents, permit, household);
    }

    private static RegistrationCaseDetailDto Map(
        RegistrationCase registrationCase,
        Person? person,
        IReadOnlyList<AdministrativeDocument> documents,
        ResidencePermit? permit,
        Household? household)
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
            registrationCase.DeclaredAddress is null
                ? null
                : new BelgianAddressDto(
                    registrationCase.DeclaredAddress.Street,
                    registrationCase.DeclaredAddress.HouseNumber,
                    registrationCase.DeclaredAddress.Box,
                    registrationCase.DeclaredAddress.PostalCode,
                    registrationCase.DeclaredAddress.Municipality),
            registrationCase.HousingSituation,
            household?.Members
                .Select(m => new HouseholdMemberDto(
                    m.Id.Value,
                    m.GivenName,
                    m.FamilyName,
                    m.BirthDate,
                    m.Role))
                .ToList() ?? [],
            person?.CivilStatus is null
                ? null
                : new CivilStatusDto(
                    person.CivilStatus.Status,
                    person.CivilStatus.SpouseGivenName,
                    person.CivilStatus.SpouseFamilyName,
                    person.CivilStatus.MarriageDate,
                    person.CivilStatus.MarriagePlace),
            documents
                .Select(d => new DocumentDto(
                    d.Id.Value,
                    d.DocumentType,
                    d.FileName,
                    d.UploadedAt))
                .ToList());
    }
}
