namespace SchaerbeekMunicipality.Web.Municipal.Components;

public sealed record NrSearchFormCriteria(
    string? GivenName,
    string? FamilyName,
    DateOnly? BirthDate,
    int Page = 1,
    int PageSize = 25);

public sealed record NationalRegisterSearchResult(
    IReadOnlyList<NationalRegisterSearchMatch> Matches,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record NationalRegisterSearchMatch(
    Guid RegisterPersonId,
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    string? BisNumber,
    string? NationalRegisterNumber,
    int MatchScore,
    string MatchReason,
    bool IsRegisteredInPopulation,
    Guid? PersonId = null)
{
    public bool CanOpenServiceCase =>
        IsRegisteredInPopulation && !string.IsNullOrWhiteSpace(NationalRegisterNumber);

    public bool CanOpenPersonFile => PersonId is not null;
}