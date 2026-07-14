using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.ListPendingPoliceVerifications;

public sealed class ListPendingPoliceVerificationsHandler(
    IPoliceVerificationRepository policeVerificationRepository,
    IRegistrationCaseRepository caseRepository,
    IChangeOfAddressCaseRepository changeOfAddressCaseRepository,
    IPersonRepository personRepository)
{
    public async Task<ListPendingPoliceVerificationsResponse> Handle(CancellationToken cancellationToken)
    {
        var pending = await policeVerificationRepository.ListPendingAsync(cancellationToken);
        var items = new List<PendingPoliceVerificationDto>(pending.Count);

        foreach (var request in pending)
            if (request.RegistrationCaseId is { } registrationCaseId)
            {
                var item = await MapRegistrationRequestAsync(request, registrationCaseId, cancellationToken);
                if (item is not null) items.Add(item);
            }
            else if (request.ChangeOfAddressCaseId is { } changeOfAddressCaseId)
            {
                var item = await MapChangeOfAddressRequestAsync(request, changeOfAddressCaseId, cancellationToken);
                if (item is not null) items.Add(item);
            }

        return new ListPendingPoliceVerificationsResponse(items, items.Count);
    }

    private async Task<PendingPoliceVerificationDto?> MapRegistrationRequestAsync(
        PoliceVerificationRequest request,
        RegistrationCaseId registrationCaseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseRepository.GetByIdAsync(registrationCaseId, cancellationToken);
        if (registrationCase is null) return null;

        Person? person = null;
        if (registrationCase.PersonId is { } personId)
            person = await personRepository.GetByIdAsync(personId, cancellationToken);

        var personName = person is null
            ? "Unknown person"
            : $"{person.GivenName} {person.FamilyName}";

        var address = registrationCase.DeclaredAddress?.FormatSingleLine() ?? "No address declared";

        return new PendingPoliceVerificationDto(
            request.Id.Value,
            registrationCase.Id.Value,
            "Registration",
            personName,
            address,
            request.RequestedAt,
            request.AttemptNumber);
    }

    private async Task<PendingPoliceVerificationDto?> MapChangeOfAddressRequestAsync(
        PoliceVerificationRequest request,
        ChangeOfAddressCaseId changeOfAddressCaseId,
        CancellationToken cancellationToken)
    {
        var changeOfAddressCase = await changeOfAddressCaseRepository.GetByIdAsync(
            changeOfAddressCaseId,
            cancellationToken);
        if (changeOfAddressCase is null) return null;

        var person = await personRepository.GetByIdAsync(changeOfAddressCase.PersonId, cancellationToken);
        var personName = person is null
            ? "Unknown person"
            : $"{person.GivenName} {person.FamilyName}";

        var address = changeOfAddressCase.NewAddress?.FormatSingleLine() ?? "No address declared";

        return new PendingPoliceVerificationDto(
            request.Id.Value,
            changeOfAddressCase.Id.Value,
            "ChangeOfAddress",
            personName,
            address,
            request.RequestedAt,
            request.AttemptNumber);
    }
}