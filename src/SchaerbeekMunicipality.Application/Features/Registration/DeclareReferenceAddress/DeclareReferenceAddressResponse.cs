namespace SchaerbeekMunicipality.Application.Features.Registration.DeclareReferenceAddress;

public sealed record DeclareReferenceAddressResponse(
    Guid CaseId,
    bool AddressDeclared,
    string FormattedAddress,
    string AddressDeclarationType);
