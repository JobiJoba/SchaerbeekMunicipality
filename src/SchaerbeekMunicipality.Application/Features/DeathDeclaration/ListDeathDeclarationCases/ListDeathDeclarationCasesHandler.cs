using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.ListDeathDeclarationCases;

public sealed record DeathDeclarationCaseListItem(
    Guid Id,
    DeathDeclarationCaseStatus Status,
    DateTimeOffset OpenedAt,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    Guid PersonId,
    string? PersonDisplayName,
    DateOnly? DeathDate,
    bool IsReadyForConfirmation);

public sealed class ListDeathDeclarationCasesHandler(
    IDeathDeclarationCaseRepository repository,
    IPersonRepository personRepository,
    DeathDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<IReadOnlyList<DeathDeclarationCaseListItem>> Handle(CancellationToken cancellationToken)
    {
        authorization.EnsureCanList(currentOfficer);

        var cases = await repository.ListAsync(cancellationToken);

        var items = new List<DeathDeclarationCaseListItem>();
        foreach (var deathCase in cases)
        {
            var person = await personRepository.GetByIdAsync(deathCase.PersonId, cancellationToken);
            var displayName = person is null
                ? null
                : $"{person.GivenName} {person.FamilyName}".Trim();

            items.Add(new DeathDeclarationCaseListItem(
                deathCase.Id.Value,
                deathCase.Status,
                deathCase.OpenedAt,
                deathCase.AssignedOfficerId?.Value,
                deathCase.LockedByOfficerId?.Value,
                deathCase.PersonId.Value,
                displayName,
                deathCase.DeathDate,
                deathCase.IsReadyForConfirmation));
        }

        return items;
    }
}
