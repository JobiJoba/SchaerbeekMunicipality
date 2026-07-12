using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.ListDocumentRequestCases;

public sealed record DocumentRequestCaseListItem(
    Guid Id,
    DocumentRequestCaseStatus Status,
    DocumentRequestType RequestType,
    DateTimeOffset RequestedAt,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    string? PersonDisplayName,
    bool PhotoAttached,
    bool FeePaid);

public sealed class ListDocumentRequestCasesHandler(
    IDocumentRequestCaseRepository repository,
    IPersonRepository personRepository,
    DocumentRequestCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<IReadOnlyList<DocumentRequestCaseListItem>> Handle(CancellationToken cancellationToken)
    {
        authorization.EnsureCanList(currentOfficer);

        var cases = await repository.ListAsync(cancellationToken);
        var items = new List<DocumentRequestCaseListItem>(cases.Count);

        foreach (var documentRequestCase in cases)
        {
            var person = await personRepository.GetByIdAsync(documentRequestCase.PersonId, cancellationToken);
            items.Add(new DocumentRequestCaseListItem(
                documentRequestCase.Id.Value,
                documentRequestCase.Status,
                documentRequestCase.RequestType,
                documentRequestCase.RequestedAt,
                documentRequestCase.AssignedOfficerId?.Value,
                documentRequestCase.LockedByOfficerId?.Value,
                FormatPersonName(person),
                documentRequestCase.PhotoAttached,
                documentRequestCase.FeePaid));
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
