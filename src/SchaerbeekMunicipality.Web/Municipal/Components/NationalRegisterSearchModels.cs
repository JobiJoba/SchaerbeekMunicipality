namespace SchaerbeekMunicipality.Web.Municipal.Components;

public sealed record NrSearchFormCriteria(
    string? GivenName,
    string? FamilyName,
    DateOnly? BirthDate);

public sealed record NationalRegisterSearchMatch(
    Guid RegisterPersonId,
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    string? BisNumber,
    string? NationalRegisterNumber,
    int MatchScore,
    string MatchReason);
