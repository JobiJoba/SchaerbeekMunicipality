namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public sealed record NewbornDetails(
    string GivenNames,
    string FamilyName,
    NewbornSex Sex,
    DateOnly DateOfBirth,
    TimeOnly? TimeOfBirth,
    string PlaceOfBirth);