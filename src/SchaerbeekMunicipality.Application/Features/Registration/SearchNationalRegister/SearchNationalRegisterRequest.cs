namespace SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

public sealed record SearchNationalRegisterRequest(
    string? GivenName,
    string? FamilyName,
    DateOnly? BirthDate,
    int Page = 1,
    int PageSize = 25,
    bool ExcludeDeceased = false);