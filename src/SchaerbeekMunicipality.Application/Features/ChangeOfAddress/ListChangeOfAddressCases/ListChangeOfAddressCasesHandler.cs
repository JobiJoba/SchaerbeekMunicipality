using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ListChangeOfAddressCases;

public sealed record ChangeOfAddressCaseListItem(
    Guid Id,
    ChangeOfAddressCaseStatus Status,
    DateTimeOffset OpenedAt,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    string? PersonDisplayName,
    bool IsReadyForConfirmation);

public sealed class ListChangeOfAddressCasesHandler(
    IChangeOfAddressCaseRepository repository,
    IPersonRepository personRepository,
    ChangeOfAddressCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<IReadOnlyList<ChangeOfAddressCaseListItem>> Handle(CancellationToken cancellationToken)
    {
        authorization.EnsureCanList(currentOfficer);

        var cases = await repository.ListAsync(cancellationToken);
        var items = new List<ChangeOfAddressCaseListItem>(cases.Count);

        foreach (var changeOfAddressCase in cases)
        {
            var person = await personRepository.GetByIdAsync(changeOfAddressCase.PersonId, cancellationToken);
            items.Add(new ChangeOfAddressCaseListItem(
                changeOfAddressCase.Id.Value,
                changeOfAddressCase.Status,
                changeOfAddressCase.OpenedAt,
                changeOfAddressCase.AssignedOfficerId?.Value,
                changeOfAddressCase.LockedByOfficerId?.Value,
                FormatPersonName(person),
                changeOfAddressCase.IsReadyForConfirmation));
        }

        return items;
    }

    private static string? FormatPersonName(Person? person)
    {
        if (person is null)
        {
            return null;
        }

        return $"{person.GivenName} {person.FamilyName}".Trim();
    }
}
