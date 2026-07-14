using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.ListRegisterAmendmentCases;

public sealed record RegisterAmendmentCaseListItem(
    Guid Id,
    RegisterAmendmentCaseStatus Status,
    AmendmentType AmendmentType,
    DateTimeOffset OpenedAt,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    string? PersonDisplayName,
    bool IsReadyForApproval);

public sealed class ListRegisterAmendmentCasesHandler(
    IRegisterAmendmentCaseRepository repository,
    IPersonRepository personRepository,
    RegisterAmendmentCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<IReadOnlyList<RegisterAmendmentCaseListItem>> Handle(CancellationToken cancellationToken)
    {
        authorization.EnsureCanList(currentOfficer);

        var cases = await repository.ListAsync(cancellationToken);
        var items = new List<RegisterAmendmentCaseListItem>(cases.Count);

        foreach (var amendmentCase in cases)
        {
            var person = await personRepository.GetByIdAsync(amendmentCase.PersonId, cancellationToken);
            items.Add(new RegisterAmendmentCaseListItem(
                amendmentCase.Id.Value,
                amendmentCase.Status,
                amendmentCase.AmendmentType,
                amendmentCase.OpenedAt,
                amendmentCase.AssignedOfficerId?.Value,
                amendmentCase.LockedByOfficerId?.Value,
                FormatPersonName(person),
                amendmentCase.IsReadyForApproval));
        }

        return items;
    }

    private static string? FormatPersonName(Person? person)
    {
        if (person is null) return null;

        return $"{person.GivenName} {person.FamilyName}".Trim();
    }
}
