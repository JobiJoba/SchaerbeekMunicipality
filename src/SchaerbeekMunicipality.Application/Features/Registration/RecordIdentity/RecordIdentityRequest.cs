namespace SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;

public sealed record RecordIdentityRequest(
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    string Nationality);

public sealed record RecordIdentityResponse(
    Guid CaseId,
    Guid PersonId,
    bool IdentityEstablished);