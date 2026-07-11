using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;

namespace SchaerbeekMunicipality.Web.Municipal;

public static class AddressFormatting
{
    public static string FormatBelgianAddress(
        string street,
        string houseNumber,
        string? box,
        string postalCode,
        string municipality) =>
        $"{street} {houseNumber}{(string.IsNullOrWhiteSpace(box) ? "" : $" bus {box}")}, {postalCode} {municipality}";

    public static string? FormatBelgianAddress(BelgianAddressDto? address) =>
        address is null
            ? null
            : FormatBelgianAddress(
                address.Street,
                address.HouseNumber,
                address.Box,
                address.PostalCode,
                address.Municipality);
}
