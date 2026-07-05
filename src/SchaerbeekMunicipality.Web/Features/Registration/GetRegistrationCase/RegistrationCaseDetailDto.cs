using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.SearchNationalRegister;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;

public sealed record RegistrationCaseChecklistDto(
    bool IdentityEstablished,
    bool LegalResidenceEstablished,
    bool AddressDeclared,
    bool AddressConfirmed,
    bool RegisterDeterminable);

public sealed record PersonDto(
    Guid Id,
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    string Nationality,
    string? BisNumber,
    string? NationalRegisterNumber,
    bool LinkedFromRegister);

public sealed record DocumentDto(
    Guid Id,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed record ResidencePermitDto(
    Guid Id,
    ResidencePermitType PermitType,
    string? CardNumber,
    DateOnly ValidFrom,
    DateOnly ValidUntil,
    string? IssuingAuthority,
    DateTimeOffset RecordedAt);

public sealed record ImmigrationDecisionDto(
    string ReferenceNumber,
    DateOnly DecisionDate);

public sealed record BelgianAddressDto(
    string Street,
    string HouseNumber,
    string? Box,
    string PostalCode,
    string Municipality);

public sealed record HouseholdMemberDto(
    Guid Id,
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    HouseholdMemberRole Role);

public sealed record CivilStatusDto(
    CivilStatus Status,
    string? SpouseGivenName,
    string? SpouseFamilyName,
    DateOnly? MarriageDate,
    string? MarriagePlace);

public sealed record PoliceVerificationDto(
    Guid RequestId,
    int AttemptNumber,
    DateTimeOffset RequestedAt,
    DateTimeOffset? CompletedAt,
    PoliceVerificationResult? Result,
    string? OfficerNotes,
    bool IsPending);

public sealed record RegistrationCaseDetailDto(
    Guid Id,
    RegistrationCaseStatus Status,
    VisitReason VisitReason,
    Guid AssignedOfficerId,
    DateTimeOffset OpenedAt,
    RegistrationCaseChecklistDto Checklist,
    PersonDto? Person,
    ResidenceCategory? ResidenceCategory,
    ResidencePermitDto? ResidencePermit,
    ImmigrationDecisionDto? ImmigrationDecision,
    BelgianAddressDto? DeclaredAddress,
    HousingSituation? HousingSituation,
    IReadOnlyList<HouseholdMemberDto> HouseholdMembers,
    CivilStatusDto? CivilStatus,
    IReadOnlyList<DocumentDto> Documents,
    IReadOnlyList<NationalRegisterMatchDto> PossibleDuplicateMatches,
    PoliceVerificationDto? ActivePoliceVerification,
    IReadOnlyList<PoliceVerificationDto> PoliceVerificationHistory);
