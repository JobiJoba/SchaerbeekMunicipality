namespace SchaerbeekMunicipality.Domain.Immigration;

public sealed record ResidencePermitDetails(
    ResidencePermitType PermitType,
    DateOnly ValidFrom,
    DateOnly ValidUntil,
    string? CardNumber,
    string? IssuingAuthority);