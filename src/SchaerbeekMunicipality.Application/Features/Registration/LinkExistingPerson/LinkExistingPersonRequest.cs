namespace SchaerbeekMunicipality.Application.Features.Registration.LinkExistingPerson;

public sealed record LinkExistingPersonRequest(Guid RegisterPersonId);

public sealed record LinkExistingPersonResponse(
    Guid CaseId,
    Guid PersonId,
    bool IdentityEstablished,
    string? BisNumber,
    string? NationalRegisterNumber,
    bool LinkedFromRegister);
