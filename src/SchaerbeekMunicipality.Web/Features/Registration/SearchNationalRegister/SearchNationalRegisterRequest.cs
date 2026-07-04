namespace SchaerbeekMunicipality.Web.Features.Registration.SearchNationalRegister;

public sealed record SearchNationalRegisterRequest(
    string? GivenName,
    string? FamilyName,
    DateOnly? BirthDate);
