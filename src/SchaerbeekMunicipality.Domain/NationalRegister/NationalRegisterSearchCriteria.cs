namespace SchaerbeekMunicipality.Domain.NationalRegister;

public readonly record struct NationalRegisterSearchCriteria(
    string? GivenName,
    string? FamilyName,
    DateOnly? BirthDate)
{
    public bool HasAnyCriterion =>
        !string.IsNullOrWhiteSpace(GivenName) ||
        !string.IsNullOrWhiteSpace(FamilyName) ||
        BirthDate.HasValue;
}
