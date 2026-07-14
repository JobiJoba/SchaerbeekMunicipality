namespace SchaerbeekMunicipality.Domain.Address;

public sealed class BelgianAddress
{
    private BelgianAddress()
    {
    }

    public string Street { get; private set; } = string.Empty;

    public string HouseNumber { get; private set; } = string.Empty;

    public string? Box { get; private set; }

    public string PostalCode { get; private set; } = string.Empty;

    public string Municipality { get; private set; } = string.Empty;

    public static BelgianAddress Create(
        string street,
        string houseNumber,
        string? box,
        string postalCode,
        string municipality)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(houseNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(municipality);

        var normalizedPostalCode = postalCode.Trim();
        if (normalizedPostalCode.Length != 4 || !normalizedPostalCode.All(char.IsDigit))
            throw new ArgumentException("Postal code must be a four-digit Belgian code.");

        if (int.Parse(normalizedPostalCode) is < 1000 or > 9999)
            throw new ArgumentException("Postal code must be between 1000 and 9999.");

        return new BelgianAddress
        {
            Street = street.Trim(),
            HouseNumber = houseNumber.Trim(),
            Box = string.IsNullOrWhiteSpace(box) ? null : box.Trim(),
            PostalCode = normalizedPostalCode,
            Municipality = municipality.Trim()
        };
    }

    public string FormatSingleLine()
    {
        var line = $"{Street} {HouseNumber}";
        if (!string.IsNullOrWhiteSpace(Box)) line += $" bus {Box}";

        return $"{line}, {PostalCode} {Municipality}";
    }
}