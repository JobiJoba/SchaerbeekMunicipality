namespace SchaerbeekMunicipality.Web.Municipal;

public static class NationalRegisterFormatting
{
    public static string FormatNumber(string value) =>
        value.Length == 11
            ? $"{value[..6]}-{value[6..9]}.{value[9..]}"
            : value;
}
