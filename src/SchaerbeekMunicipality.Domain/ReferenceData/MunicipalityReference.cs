namespace SchaerbeekMunicipality.Domain.ReferenceData;

public sealed class MunicipalityReference
{
    private MunicipalityReference()
    {
    }

    public string PostalCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public static MunicipalityReference Create(string postalCode, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new MunicipalityReference
        {
            PostalCode = postalCode.Trim(),
            Name = name.Trim(),
        };
    }
}
