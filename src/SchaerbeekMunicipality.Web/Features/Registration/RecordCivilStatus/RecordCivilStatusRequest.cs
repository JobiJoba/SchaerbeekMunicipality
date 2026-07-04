using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordCivilStatus;

public sealed record RecordCivilStatusRequest(
    CivilStatus Status,
    string? SpouseGivenName,
    string? SpouseFamilyName,
    DateOnly? MarriageDate,
    string? MarriagePlace);

public sealed record RecordCivilStatusResponse(
    Guid CaseId,
    CivilStatus Status,
    string? SpouseGivenName,
    string? SpouseFamilyName,
    DateOnly? MarriageDate,
    string? MarriagePlace);
