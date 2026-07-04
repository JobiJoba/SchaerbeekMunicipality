namespace SchaerbeekMunicipality.Domain.NationalRegister;

public readonly record struct NationalRegisterNumber(string Value)
{
    public static NationalRegisterNumber Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalized = value.Trim().Replace(".", string.Empty).Replace("-", string.Empty);
        if (normalized.Length != 11 || !normalized.All(char.IsDigit))
        {
            throw new ArgumentException("National Register number must be an 11-digit identifier.");
        }

        if (!IsValidCheckDigits(normalized))
        {
            throw new ArgumentException("National Register number has invalid check digits.");
        }

        return new NationalRegisterNumber(normalized);
    }

    public static NationalRegisterNumber GenerateStub(DateOnly birthDate, int sequence)
    {
        if (sequence is < 1 or > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(sequence), "Sequence must be between 1 and 999.");
        }

        var baseNumber = $"{birthDate:yyMMdd}{sequence:D3}";
        var check = 97 - (int.Parse(baseNumber) % 97);
        var full = $"{baseNumber}{check:D2}";

        return new NationalRegisterNumber(full);
    }

    public string Format() =>
        $"{Value[..6]}-{Value[6..9]}.{Value[9..]}";

    public override string ToString() => Value;

    private static bool IsValidCheckDigits(string normalized)
    {
        var basePart = normalized[..9];
        var checkPart = normalized[9..];
        var expected = 97 - (int.Parse(basePart) % 97);
        return int.Parse(checkPart) == expected;
    }
}
