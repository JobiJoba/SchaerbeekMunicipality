using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.PersonFile.GetPersonFile;

public sealed record PersonFileHeaderDto(
    Guid PersonId,
    string GivenName,
    string FamilyName,
    string Initials,
    string? NationalRegisterNumber,
    string? BisNumber,
    RegisterTarget? RegisterTarget);

public sealed record PersonFileDomicileDto(
    BelgianAddressDto? CurrentAddress,
    string? HousingSituation);

public sealed record PersonFileCertificateDto(
    Guid Id,
    string CertificateType,
    string ReferenceNumber,
    DateTimeOffset IssuedAt,
    Guid RegistrationCaseId);

public sealed record PersonFileHouseholdMemberDto(
    Guid? PersonId,
    string GivenName,
    string FamilyName,
    DateOnly? BirthDate,
    string Role,
    string Source);

public sealed record PersonFileAddressDto(
    string Street,
    string HouseNumber,
    string? Box,
    string PostalCode,
    string Municipality,
    string? HousingSituation,
    DateTimeOffset? EffectiveFrom,
    bool IsCurrent,
    string Source);

public sealed record PersonFileCaseDto(
    Guid CaseId,
    string Workflow,
    string Status,
    DateTimeOffset OpenedAt,
    DateTimeOffset? ClosedAt,
    string DetailPath);

public sealed record PersonFileHistoryEventDto(
    string Title,
    DateTimeOffset Timestamp,
    string? Description,
    string Source);

public sealed record GetPersonFileResponse(
    PersonFileHeaderDto Header,
    PersonDto Identity,
    CivilStatusDto? CivilStatus,
    PersonFileDomicileDto Domicile,
    IReadOnlyList<PersonFileHouseholdMemberDto> HouseholdMembers,
    IReadOnlyList<PersonFileAddressDto> Addresses,
    IReadOnlyList<PersonFileCaseDto> Cases,
    IReadOnlyList<PersonFileCertificateDto> Certificates,
    IReadOnlyList<PersonFileHistoryEventDto> History);