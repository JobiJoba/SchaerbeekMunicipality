namespace SchaerbeekMunicipality.Application.Features.PersonFile.ListPersonCases;

public sealed record ListPersonCasesResponse(
    IReadOnlyList<PersonFileCaseListItemDto> Cases);

public sealed record PersonFileCaseListItemDto(
    Guid CaseId,
    string Workflow,
    string Status,
    DateTimeOffset OpenedAt,
    DateTimeOffset? ClosedAt,
    string DetailPath);