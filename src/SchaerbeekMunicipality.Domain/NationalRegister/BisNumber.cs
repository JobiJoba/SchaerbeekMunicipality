namespace SchaerbeekMunicipality.Domain.NationalRegister;

public readonly record struct BisNumber(string Value)
{
    public static BisNumber Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalized = value.Trim().Replace(".", string.Empty).Replace("-", string.Empty);
        if (normalized.Length != 11 || !normalized.All(char.IsDigit))
        {
            throw new ArgumentException("BIS number must be an 11-digit identifier.");
        }

        if (normalized[0] != '7')
        {
            throw new ArgumentException("BIS numbers in this simulation start with digit 7.");
        }

        return new BisNumber(normalized);
    }

    public string Format() =>
        $"{Value[..6]}-{Value[6..9]}.{Value[9..]}";

    public override string ToString() => Value;
}
