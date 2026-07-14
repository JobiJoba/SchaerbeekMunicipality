namespace SchaerbeekMunicipality.Domain.Address;

public sealed record AddressDetails(
    string Street,
    string HouseNumber,
    string? Box,
    string PostalCode,
    string Municipality);