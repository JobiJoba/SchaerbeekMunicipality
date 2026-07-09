using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.ListBirthDeclarationCases;

public sealed record BirthDeclarationCaseListItem(
    Guid Id,
    BirthDeclarationCaseStatus Status,
    DateTimeOffset OpenedAt,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    string? ChildDisplayName,
    bool IsReadyForConfirmation);

public sealed class ListBirthDeclarationCasesHandler(
    IBirthDeclarationCaseRepository repository,
    BirthDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<IReadOnlyList<BirthDeclarationCaseListItem>> Handle(CancellationToken cancellationToken)
    {
        authorization.EnsureCanList(currentOfficer);

        var cases = await repository.ListAsync(cancellationToken);

        return cases
            .Select(c => new BirthDeclarationCaseListItem(
                c.Id.Value,
                c.Status,
                c.OpenedAt,
                c.AssignedOfficerId?.Value,
                c.LockedByOfficerId?.Value,
                FormatChildName(c),
                c.IsReadyForConfirmation))
            .ToList();
    }

    private static string? FormatChildName(BirthDeclarationCase birthDeclarationCase)
    {
        if (string.IsNullOrWhiteSpace(birthDeclarationCase.ChildGivenNames))
        {
            return null;
        }

        return $"{birthDeclarationCase.ChildGivenNames} {birthDeclarationCase.ChildFamilyName}".Trim();
    }
}
