namespace SchaerbeekMunicipality.Domain.ReferenceData;

public sealed class StreetReference
{
    private StreetReference()
    {
    }

    public int Id { get; private set; }

    public string PostalCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public static StreetReference Create(int id, string postalCode, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new StreetReference
        {
            Id = id,
            PostalCode = postalCode.Trim(),
            Name = name.Trim(),
        };
    }
}
