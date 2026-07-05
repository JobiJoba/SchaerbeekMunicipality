using SchaerbeekMunicipality.Domain.Immigration;

namespace SchaerbeekMunicipality.Domain.Registration;

public static class RegisterTargetResolver
{
    public static RegisterTarget? Suggest(ResidenceCategory? category, string? nationality)
    {
        if (category is null)
        {
            return null;
        }

        return category switch
        {
            ResidenceCategory.EuCitizen when IsBelgian(nationality) => RegisterTarget.PopulationRegister,
            ResidenceCategory.EuCitizen => RegisterTarget.ForeignersRegister,
            ResidenceCategory.NonEuWorker => RegisterTarget.ForeignersRegister,
            ResidenceCategory.Student => RegisterTarget.ForeignersRegister,
            _ => null,
        };
    }

    public static bool IsAllowed(
        ResidenceCategory? category,
        string? nationality,
        RegisterTarget target)
    {
        var suggested = Suggest(category, nationality);
        return suggested == target;
    }

    private static bool IsBelgian(string? nationality) =>
        nationality?.Trim().Equals("Belgian", StringComparison.OrdinalIgnoreCase) == true;
}
