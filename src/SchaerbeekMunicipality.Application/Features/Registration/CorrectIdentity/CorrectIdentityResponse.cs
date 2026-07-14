namespace SchaerbeekMunicipality.Application.Features.Registration.CorrectIdentity;

public sealed record CorrectIdentityResponse(
    Guid CaseId,
    Guid PersonId,
    bool IdentityEstablished,
    bool LegalResidenceEstablished,
    string? PolicyMessage);