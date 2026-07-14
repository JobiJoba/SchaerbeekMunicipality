namespace SchaerbeekMunicipality.Domain.NationalRegister;

public sealed record NationalRegisterMatch(
    NationalRegisterPersonId RegisterPersonId,
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    string Nationality,
    string? BisNumber,
    string? NationalRegisterNumber,
    int MatchScore,
    string MatchReason);