using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordCivilStatus;

public sealed record RecordCivilStatusRequest(
    CivilStatus Status,
    string? SpouseGivenName,
    string? SpouseFamilyName,
    DateOnly? MarriageDate,
    string? MarriagePlace,
    MarriageRecognitionStatus MarriageRecognitionStatus = MarriageRecognitionStatus.NotApplicable);

public sealed record RecordCivilStatusResponse(
    Guid CaseId,
    CivilStatus Status,
    CivilStatus EffectiveRegisterStatus,
    string? SpouseGivenName,
    string? SpouseFamilyName,
    DateOnly? MarriageDate,
    string? MarriagePlace,
    MarriageRecognitionStatus MarriageRecognitionStatus,
    bool MarriageRecognitionBlocking);