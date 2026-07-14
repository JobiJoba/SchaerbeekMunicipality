namespace SchaerbeekMunicipality.Application.Features.PersonFile.SearchPersonFile;

public sealed record PersonFileSearchMatchDto(
    Guid RegisterPersonId,
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    string Nationality,
    string? BisNumber,
    string? NationalRegisterNumber,
    int MatchScore,
    string MatchReason,
    bool IsRegisteredInPopulation,
    Guid? PersonId,
    bool CanOpenPersonFile);

public sealed record SearchPersonFileResponse(
    IReadOnlyList<PersonFileSearchMatchDto> Matches,
    int TotalCount,
    int Page,
    int PageSize);