namespace SchaerbeekMunicipality.Domain.Identity;

public sealed record CivilStatusDetails(
    CivilStatus Status,
    string? SpouseGivenName,
    string? SpouseFamilyName,
    DateOnly? MarriageDate,
    string? MarriagePlace,
    MarriageRecognitionStatus MarriageRecognitionStatus = MarriageRecognitionStatus.NotApplicable);
