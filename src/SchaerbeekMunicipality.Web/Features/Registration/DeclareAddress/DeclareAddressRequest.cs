namespace SchaerbeekMunicipality.Web.Features.Registration.DeclareAddress;

public sealed record DeclareAddressRequest(
    string Street,
    string HouseNumber,
    string? Box,
    string PostalCode,
    string Municipality);

public sealed record DeclareAddressResponse(
    Guid CaseId,
    bool AddressDeclared,
    string FormattedAddress);
