using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.PersonFile;

namespace SchaerbeekMunicipality.Application.Features.PersonFile.ListPersonCases;

public sealed class ListPersonCasesHandler(
    PersonFileAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository,
    IPersonFileQuery personFileQuery)
{
    public async Task<ListPersonCasesResponse> Handle(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        _ = await personRepository.GetByIdAsync(personId, cancellationToken)
            ?? throw new KeyNotFoundException("Person not found.");

        var cases = await personFileQuery.ListCasesByPersonIdAsync(personId, cancellationToken);

        return new ListPersonCasesResponse(cases.Select(c => new PersonFileCaseListItemDto(
            c.CaseId,
            c.Workflow,
            c.Status,
            c.OpenedAt,
            c.ClosedAt,
            c.DetailPath)).ToList());
    }
}