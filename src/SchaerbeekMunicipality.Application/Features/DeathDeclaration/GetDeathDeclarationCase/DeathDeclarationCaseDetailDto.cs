using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.GetDeathDeclarationCase;

public sealed record DeathDeclarationDocumentDto(
    Guid Id,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed record DeathDeclarationCaseDetailDto(
    Guid Id,
    DeathDeclarationCaseStatus Status,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool CanEdit,
    bool IsReadOnlyDueToLock,
    Guid PersonId,
    string? PersonGivenName,
    string? PersonFamilyName,
    string? PersonNationalRegisterNumber,
    DateOnly? PersonBirthDate,
    DateOnly? DeathDate,
    string? DeathPlace,
    bool DeathAbroad,
    InformantRelationship? InformantRelationship,
    IReadOnlyList<DeathDeclarationDocumentDto> Documents,
    DateTimeOffset? HouseholdReviewedAt,
    bool PersonIdentified,
    bool DeathFactsRecorded,
    bool DeathActAttached,
    bool HouseholdReviewed,
    bool ReadyForConfirmation,
    DateTimeOffset OpenedAt,
    DateTimeOffset? ConfirmedAt,
    DeathDeclarationRejectionReason? RejectionReason,
    SuspensionReason? SuspensionReason,
    string? DecisionNotes);
