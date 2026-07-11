using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.GetBirthDeclarationCase;

public sealed record BirthDeclarationParentDto(
    Guid PersonId,
    string GivenName,
    string FamilyName,
    string? NationalRegisterNumber,
    ParentRole Role);

public sealed record BirthDeclarationDocumentDto(
    Guid Id,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed record BirthDeclarationCaseDetailDto(
    Guid Id,
    BirthDeclarationCaseStatus Status,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool CanEdit,
    bool IsReadOnlyDueToLock,
    string? ChildGivenNames,
    string? ChildFamilyName,
    NewbornSex? ChildSex,
    DateOnly? ChildDateOfBirth,
    TimeOnly? ChildTimeOfBirth,
    string? ChildPlaceOfBirth,
    IReadOnlyList<BirthDeclarationParentDto> Parents,
    IReadOnlyList<BirthDeclarationDocumentDto> Documents,
    string? HouseholdAddress,
    bool ChildDetailsRecorded,
    bool AtLeastOneParentLinked,
    bool MedicalDeclarationAttached,
    bool HouseholdEstablished,
    bool ReadyForConfirmation,
    DateTimeOffset OpenedAt,
    DateTimeOffset? ConfirmedAt,
    string? ChildNationalRegisterNumber,
    BirthDeclarationRejectionReason? RejectionReason,
    SuspensionReason? SuspensionReason,
    string? DecisionNotes);
