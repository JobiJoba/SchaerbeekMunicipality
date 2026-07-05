using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;
using SchaerbeekMunicipality.Web.Features.Registration.SearchNationalRegister;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;

public sealed class GetRegistrationCaseHandler(
    RegistrationCaseGuard caseGuard,
    RegistrationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository,
    IResidencePermitRepository permitRepository,
    IHouseholdRepository householdRepository,
    INationalRegisterRepository nationalRegisterRepository,
    IPoliceVerificationRepository policeVerificationRepository)
{
    public async Task<RegistrationCaseDetailDto?> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var registrationCase = await caseGuard.GetForViewAsync(caseId, cancellationToken);

        Person? person = null;
        if (registrationCase.PersonId is { } personId)
        {
            person = await personRepository.GetByIdAsync(personId, cancellationToken);
        }

        var documents = await documentRepository.ListByCaseIdAsync(caseId, cancellationToken);
        var permit = await permitRepository.GetByCaseIdAsync(caseId, cancellationToken);
        var household = await householdRepository.GetByCaseIdAsync(caseId, cancellationToken);
        var possibleDuplicates = await FindPossibleDuplicatesAsync(person, cancellationToken);
        var activePoliceVerification = await policeVerificationRepository.GetPendingByCaseIdAsync(
            caseId,
            cancellationToken);
        var policeVerifications = await policeVerificationRepository.ListByCaseIdAsync(caseId, cancellationToken);

        return Map(
            registrationCase,
            person,
            documents,
            permit,
            household,
            possibleDuplicates,
            activePoliceVerification,
            policeVerifications);
    }

    private async Task<IReadOnlyList<NationalRegisterMatchDto>> FindPossibleDuplicatesAsync(
        Person? person,
        CancellationToken cancellationToken)
    {
        if (person is null || person.LinkedRegisterRecordId is not null)
        {
            return [];
        }

        var criteria = new NationalRegisterSearchCriteria(
            person.GivenName,
            person.FamilyName,
            person.BirthDate);

        var matches = await nationalRegisterRepository.SearchAsync(criteria, cancellationToken);

        return matches
            .Where(m => m.MatchScore >= NationalRegisterSearchScorer.ExactMatchScore - 20)
            .Select(m => new NationalRegisterMatchDto(
                m.RegisterPersonId.Value,
                m.GivenName,
                m.FamilyName,
                m.BirthDate,
                m.Nationality,
                m.BisNumber,
                m.NationalRegisterNumber,
                m.MatchScore,
                m.MatchReason))
            .ToList();
    }

    private RegistrationCaseDetailDto Map(
        RegistrationCase registrationCase,
        Person? person,
        IReadOnlyList<AdministrativeDocument> documents,
        ResidencePermit? permit,
        Household? household,
        IReadOnlyList<NationalRegisterMatchDto> possibleDuplicates,
        PoliceVerificationRequest? activePoliceVerification,
        IReadOnlyList<PoliceVerificationRequest> policeVerifications)
    {
        var officerId = OfficerId.From(currentOfficer.OfficerId);
        var canEdit = authorization.CanEditCase(currentOfficer.Role, registrationCase, officerId);
        var isReadOnlyDueToLock = authorization.IsReadOnlyDueToLock(
            currentOfficer.Role,
            registrationCase,
            officerId);

        var checklist = registrationCase.Checklist;
        var suggested = RegisterTargetResolver.Suggest(
            registrationCase.ResidenceCategory,
            person?.Nationality);

        return new RegistrationCaseDetailDto(
            registrationCase.Id.Value,
            registrationCase.Status,
            registrationCase.VisitReason,
            registrationCase.AssignedOfficerId?.Value,
            registrationCase.LockedByOfficerId?.Value,
            registrationCase.LockedAt,
            canEdit,
            isReadOnlyDueToLock,
            registrationCase.OpenedAt,
            registrationCase.ClosedAt,
            new RegistrationCaseChecklistDto(
                checklist.IdentityEstablished,
                checklist.LegalResidenceEstablished,
                checklist.AddressDeclared,
                checklist.AddressConfirmed,
                checklist.RegisterDeterminable),
            registrationCase.IsReadyForApproval,
            suggested?.ToString(),
            registrationCase.SelectedRegisterTarget,
            registrationCase.RejectionReason,
            registrationCase.SuspensionReason,
            registrationCase.DecisionNotes,
            person is null
                ? null
                : new PersonDto(
                    person.Id.Value,
                    person.GivenName,
                    person.FamilyName,
                    person.BirthDate,
                    person.Nationality,
                    person.BisNumber?.Value,
                    person.NationalRegisterNumber?.Value,
                    person.LinkedRegisterRecordId is not null),
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
                .ToList(),
            possibleDuplicates,
            activePoliceVerification is null ? null : MapPoliceVerification(activePoliceVerification),
            policeVerifications
                .Where(v => !v.IsPending)
                .Select(MapPoliceVerification)
                .ToList());
    }

    private static PoliceVerificationDto MapPoliceVerification(PoliceVerificationRequest request) =>
        new(
            request.Id.Value,
            request.AttemptNumber,
            request.RequestedAt,
            request.CompletedAt,
            request.Result,
            request.OfficerNotes,
            request.IsPending);
}
