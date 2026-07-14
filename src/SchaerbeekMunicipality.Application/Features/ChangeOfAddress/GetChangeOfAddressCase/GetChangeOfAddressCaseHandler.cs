using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.GetChangeOfAddressCase;

public sealed class GetChangeOfAddressCaseHandler(
    ChangeOfAddressCaseGuard caseGuard,
    ChangeOfAddressCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository,
    IPoliceVerificationRepository policeVerificationRepository)
{
    public async Task<ChangeOfAddressCaseDetailDto?> Handle(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var changeOfAddressCase = await caseGuard.GetForViewAsync(caseId, cancellationToken);
        var person = await personRepository.GetByIdAsync(changeOfAddressCase.PersonId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{changeOfAddressCase.PersonId}' was not found.");

        var documents = await documentRepository.ListByChangeOfAddressCaseIdAsync(caseId, cancellationToken);

        var householdMembers = new List<ChangeOfAddressHouseholdMemberDto>();
        foreach (var link in changeOfAddressCase.HouseholdMemberLinks)
        {
            var member = await personRepository.GetByIdAsync(link.PersonId, cancellationToken);
            if (member is null) continue;

            householdMembers.Add(new ChangeOfAddressHouseholdMemberDto(
                member.Id.Value,
                member.GivenName,
                member.FamilyName,
                member.NationalRegisterNumber?.Value));
        }

        var activePoliceVerification = await policeVerificationRepository.GetPendingByChangeOfAddressCaseIdAsync(
            caseId,
            cancellationToken);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        var canEdit = authorization.CanEditCase(currentOfficer.Role, changeOfAddressCase, officerId);
        var isReadOnlyDueToLock = authorization.IsReadOnlyDueToLock(
            currentOfficer.Role,
            changeOfAddressCase,
            officerId);

        var checklist = changeOfAddressCase.Checklist;

        return new ChangeOfAddressCaseDetailDto(
            changeOfAddressCase.Id.Value,
            changeOfAddressCase.Status,
            changeOfAddressCase.AssignedOfficerId?.Value,
            changeOfAddressCase.LockedByOfficerId?.Value,
            changeOfAddressCase.LockedAt,
            canEdit,
            isReadOnlyDueToLock,
            person.Id.Value,
            person.GivenName,
            person.FamilyName,
            person.NationalRegisterNumber?.Value,
            MapAddress(changeOfAddressCase.PreviousAddress),
            MapAddress(changeOfAddressCase.NewAddress),
            changeOfAddressCase.HousingSituation,
            changeOfAddressCase.EffectiveDate,
            documents.Select(d => new ChangeOfAddressDocumentDto(
                d.Id.Value,
                d.DocumentType,
                d.FileName,
                d.UploadedAt)).ToList(),
            householdMembers,
            checklist.PersonIdentified,
            checklist.NewAddressDeclared,
            checklist.HousingDocumentRequired,
            checklist.HousingDocumentAttached,
            checklist.PoliceVerificationRequested,
            checklist.PoliceVerificationPositive,
            changeOfAddressCase.IsReadyForConfirmation,
            changeOfAddressCase.OpenedAt,
            changeOfAddressCase.ConfirmedAt,
            changeOfAddressCase.ClosedAt,
            changeOfAddressCase.RejectionReason,
            changeOfAddressCase.DecisionNotes,
            activePoliceVerification is null ? null : MapPoliceVerification(activePoliceVerification));
    }

    private static BelgianAddressDto? MapAddress(BelgianAddress? address)
    {
        return address is null
            ? null
            : new BelgianAddressDto(
                address.Street,
                address.HouseNumber,
                address.Box,
                address.PostalCode,
                address.Municipality);
    }

    private static PoliceVerificationDto MapPoliceVerification(PoliceVerificationRequest request)
    {
        return new PoliceVerificationDto(
            request.Id.Value,
            request.AttemptNumber,
            request.RequestedAt,
            request.CompletedAt,
            request.Result,
            request.OfficerNotes,
            request.IsPending);
    }
}