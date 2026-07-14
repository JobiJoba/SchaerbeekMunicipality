using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.GetChangeOfAddressCase;

public sealed record ChangeOfAddressHouseholdMemberDto(
    Guid PersonId,
    string GivenName,
    string FamilyName,
    string? NationalRegisterNumber);

public sealed record ChangeOfAddressDocumentDto(
    Guid Id,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed record ChangeOfAddressCaseDetailDto(
    Guid Id,
    ChangeOfAddressCaseStatus Status,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool CanEdit,
    bool IsReadOnlyDueToLock,
    Guid PersonId,
    string PersonGivenName,
    string PersonFamilyName,
    string? PersonNationalRegisterNumber,
    BelgianAddressDto? PreviousAddress,
    BelgianAddressDto? NewAddress,
    HousingSituation? HousingSituation,
    DateOnly? EffectiveDate,
    IReadOnlyList<ChangeOfAddressDocumentDto> Documents,
    IReadOnlyList<ChangeOfAddressHouseholdMemberDto> HouseholdMembers,
    bool PersonIdentified,
    bool NewAddressDeclared,
    bool HousingDocumentRequired,
    bool HousingDocumentAttached,
    bool PoliceVerificationRequested,
    bool PoliceVerificationPositive,
    bool ReadyForConfirmation,
    DateTimeOffset OpenedAt,
    DateTimeOffset? ConfirmedAt,
    DateTimeOffset? ClosedAt,
    ChangeOfAddressRejectionReason? RejectionReason,
    string? DecisionNotes,
    PoliceVerificationDto? ActivePoliceVerification);