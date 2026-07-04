namespace SchaerbeekMunicipality.Web.Features.Registration.SearchNationalRegister;

public sealed record NationalRegisterMatchDto(
    Guid RegisterPersonId,
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    string Nationality,
    string? BisNumber,
    string? NationalRegisterNumber,
    int MatchScore,
    string MatchReason);

public sealed record SearchNationalRegisterResponse(
    IReadOnlyList<NationalRegisterMatchDto> Matches);
