using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ListPendingPoliceVerifications;

public sealed class ListPendingPoliceVerificationsHandler(
    IPoliceVerificationRepository policeVerificationRepository,
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository)
{
    public async Task<ListPendingPoliceVerificationsResponse> Handle(CancellationToken cancellationToken)
    {
        var pending = await policeVerificationRepository.ListPendingAsync(cancellationToken);
        var items = new List<PendingPoliceVerificationDto>(pending.Count);

        foreach (var request in pending)
        {
            var registrationCase = await caseRepository.GetByIdAsync(request.RegistrationCaseId, cancellationToken);
            if (registrationCase is null)
            {
                continue;
            }

            Person? person = null;
            if (registrationCase.PersonId is { } personId)
            {
                person = await personRepository.GetByIdAsync(personId, cancellationToken);
            }

            var personName = person is null
                ? "Unknown person"
                : $"{person.GivenName} {person.FamilyName}";

            var address = registrationCase.DeclaredAddress?.FormatSingleLine() ?? "No address declared";

            items.Add(new PendingPoliceVerificationDto(
                request.Id.Value,
                request.RegistrationCaseId.Value,
                personName,
                address,
                request.RequestedAt,
                request.AttemptNumber));
        }

        return new ListPendingPoliceVerificationsResponse(items, items.Count);
    }
}
