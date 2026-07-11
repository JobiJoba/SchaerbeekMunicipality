using SchaerbeekMunicipality.Domain.Address;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.DeclareNewAddress;

public sealed record DeclareNewAddressRequest(
    string Street,
    string HouseNumber,
    string? Box,
    string PostalCode,
    string Municipality,
    HousingSituation HousingSituation,
    DateOnly EffectiveDate);

public sealed record DeclareNewAddressResponse(
    Guid CaseId,
    string FormattedAddress,
    bool NewAddressDeclared,
    bool HousingDocumentRequired);
