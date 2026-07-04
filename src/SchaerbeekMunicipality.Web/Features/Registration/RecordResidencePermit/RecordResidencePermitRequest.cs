using SchaerbeekMunicipality.Domain.Immigration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordResidencePermit;

public sealed record RecordResidencePermitRequest(
    ResidencePermitType PermitType,
    DateOnly ValidFrom,
    DateOnly ValidUntil,
    string? CardNumber,
    string? IssuingAuthority);

public sealed record RecordResidencePermitResponse(
    Guid CaseId,
    Guid PermitId,
    bool LegalResidenceEstablished,
    string? PolicyMessage);
