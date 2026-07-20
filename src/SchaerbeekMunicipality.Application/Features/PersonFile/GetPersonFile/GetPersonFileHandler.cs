using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Domain.Certificates;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Common;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.PersonFile;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.PersonFile.GetPersonFile;

public sealed class GetPersonFileHandler(
    PersonFileAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository,
    IPersonFileQuery personFileQuery,
    ICertificateRequestRepository certificateRepository,
    IRegistrationCaseRepository registrationCaseRepository,
    IChangeOfAddressCaseRepository changeOfAddressCaseRepository)
{
    public Task<GetPersonFileResponse> Handle(PersonId personId, CancellationToken cancellationToken)
    {
        return BuildResponseAsync(personId, cancellationToken);
    }

    public async Task<GetPersonFileResponse> HandleByNationalRegisterNumber(
        string nationalRegisterNumber,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        NationalRegisterNumber nr;
        try
        {
            nr = NationalRegisterNumber.Create(nationalRegisterNumber);
        }
        catch (ArgumentException ex)
        {
            throw new KeyNotFoundException("Person not found.", ex);
        }

        var person = await personRepository.GetByNationalRegisterNumberAsync(nr, cancellationToken)
                     ?? throw new KeyNotFoundException("Person not found.");

        return await BuildResponseAsync(person.Id, cancellationToken);
    }

    private async Task<GetPersonFileResponse> BuildResponseAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var person = await personRepository.GetByIdAsync(personId, cancellationToken)
                     ?? throw new KeyNotFoundException("Person not found.");

        var registerTarget = await personFileQuery.GetRegisterTargetAsync(personId, cancellationToken);
        var cases = await personFileQuery.ListCasesByPersonIdAsync(personId, cancellationToken);
        var householdMembers = await personFileQuery.GetHouseholdMembersAsync(personId, cancellationToken);
        var addresses = await personFileQuery.ListAddressHistoryAsync(personId, cancellationToken);
        var history = await personFileQuery.ListHistoryEventsAsync(personId, cancellationToken);
        var documents = await personFileQuery.ListDocumentsByPersonIdAsync(personId, cancellationToken);
        var certificates = await certificateRepository.ListByPersonIdAsync(personId, cancellationToken);
        var housingSituation = await ResolveHousingSituationAsync(personId, cancellationToken);

        var header = new PersonFileHeaderDto(
            person.Id.Value,
            person.GivenName,
            person.FamilyName,
            BuildInitials(person.GivenName, person.FamilyName),
            person.NationalRegisterNumber?.Value,
            person.BisNumber?.Value,
            registerTarget);

        var identity = new PersonDto(
            person.Id.Value,
            person.GivenName,
            person.FamilyName,
            person.BirthDate,
            person.Nationality,
            person.BisNumber?.Value,
            person.NationalRegisterNumber?.Value,
            person.LinkedRegisterRecordId is not null,
            person.BirthPlace is null
                ? null
                : new BirthInformationDto(person.BirthPlace, person.BirthCountry),
            person.IsDeceased,
            person.DateOfDeath);

        var civilStatus = person.CivilStatus is null
            ? null
            : new CivilStatusDto(
                person.CivilStatus.Status,
                person.CivilStatus.EffectiveRegisterStatus,
                person.CivilStatus.SpouseGivenName,
                person.CivilStatus.SpouseFamilyName,
                person.CivilStatus.MarriageDate,
                person.CivilStatus.MarriagePlace,
                person.CivilStatus.MarriageRecognitionStatus);

        var domicile = new PersonFileDomicileDto(
            person.DomicileAddress is null
                ? null
                : new BelgianAddressDto(
                    person.DomicileAddress.Street,
                    person.DomicileAddress.HouseNumber,
                    person.DomicileAddress.Box,
                    person.DomicileAddress.PostalCode,
                    person.DomicileAddress.Municipality),
            housingSituation);

        return new GetPersonFileResponse(
            header,
            identity,
            civilStatus,
            domicile,
            householdMembers.Select(m => new PersonFileHouseholdMemberDto(
                m.PersonId,
                m.GivenName,
                m.FamilyName,
                m.BirthDate,
                m.Role,
                m.Source)).ToList(),
            addresses.Select(a => new PersonFileAddressDto(
                a.Street,
                a.HouseNumber,
                a.Box,
                a.PostalCode,
                a.Municipality,
                a.HousingSituation,
                a.EffectiveFrom,
                a.IsCurrent,
                a.Source)).ToList(),
            cases.Select(c => new PersonFileCaseDto(
                c.CaseId,
                c.Workflow,
                c.Status,
                c.OpenedAt,
                c.ClosedAt,
                c.DetailPath)).ToList(),
            certificates.Select(c => new PersonFileCertificateDto(
                c.Id.Value,
                c.CertificateType.ToString(),
                c.ReferenceNumber,
                c.IssuedAt,
                c.RegistrationCaseId.Value)).ToList(),
            history.Select(h => new PersonFileHistoryEventDto(
                h.Title,
                h.Timestamp,
                h.Description,
                h.Source)).ToList(),
            documents.Select(d => new PersonFileDocumentDto(
                d.DocumentId,
                d.CaseId,
                d.Workflow,
                d.DocumentType.ToDisplayString(),
                d.FileName,
                d.UploadedAt,
                PersonFileDocumentPaths.BuildDownloadPath(d.Workflow, d.CaseId, d.DocumentId),
                d.CaseDetailPath)).ToList());
    }

    private async Task<string?> ResolveHousingSituationAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var coaCases = await changeOfAddressCaseRepository.ListAsync(cancellationToken);
        var latestConfirmedCoa = coaCases
            .Where(c => c.PersonId == personId && c.Status == ChangeOfAddressCaseStatus.Confirmed)
            .OrderByDescending(c => c.ConfirmedAt ?? c.OpenedAt)
            .FirstOrDefault();

        if (latestConfirmedCoa?.HousingSituation is { } coaHousing) return coaHousing.ToString();

        var registrationCase = await registrationCaseRepository.GetLatestRegisteredByPersonIdAsync(
            personId,
            cancellationToken);

        return registrationCase?.HousingSituation?.ToString();
    }

    private static string BuildInitials(string givenName, string familyName)
    {
        var givenInitial = string.IsNullOrWhiteSpace(givenName) ? string.Empty : givenName[..1];
        var familyInitial = string.IsNullOrWhiteSpace(familyName) ? string.Empty : familyName[..1];
        return $"{givenInitial}{familyInitial}".ToUpperInvariant();
    }
}