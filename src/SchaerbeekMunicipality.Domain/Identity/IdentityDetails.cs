namespace SchaerbeekMunicipality.Domain.Identity;

public sealed record IdentityDetails(
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    string Nationality);
