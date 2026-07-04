using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;

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
    string Nationality);

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
    IReadOnlyList<DocumentDto> Documents);
