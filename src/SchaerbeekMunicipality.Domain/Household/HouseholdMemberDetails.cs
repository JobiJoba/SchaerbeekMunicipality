namespace SchaerbeekMunicipality.Domain.Household;

public sealed record HouseholdMemberDetails(
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    HouseholdMemberRole Role);
