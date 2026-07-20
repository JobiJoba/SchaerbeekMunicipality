namespace SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

public sealed record NationalRegisterMatchDto(
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
    bool IsDeceased = false);

public sealed record SearchNationalRegisterResponse(
    IReadOnlyList<NationalRegisterMatchDto> Matches,
    int TotalCount,
    int Page,
    int PageSize);