using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
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

public sealed record RegistrationCaseDetailDto(
    Guid Id,
    string Status,
    VisitReason VisitReason,
    Guid AssignedOfficerId,
    DateTimeOffset OpenedAt,
    RegistrationCaseChecklistDto Checklist,
    PersonDto? Person,
    IReadOnlyList<DocumentDto> Documents);
