using SchaerbeekMunicipality.Domain.Immigration;

namespace SchaerbeekMunicipality.Domain.Registration;

public static class RegisterTargetResolver
{
    public static RegisterTarget? Suggest(
        ResidenceCategory? category,
        string? nationality,
        bool hasImmigrationDecision = false)
    {
        if (category is null) return null;

        return category switch
        {
            ResidenceCategory.EuCitizen when IsBelgian(nationality) => RegisterTarget.PopulationRegister,
            ResidenceCategory.EuCitizen => RegisterTarget.ForeignersRegister,
            ResidenceCategory.NonEuWorker => RegisterTarget.ForeignersRegister,
            ResidenceCategory.Student => RegisterTarget.ForeignersRegister,
            ResidenceCategory.Refugee when hasImmigrationDecision => RegisterTarget.WaitingRegister,
            _ => null
        };
    }

    public static bool IsAllowed(
        ResidenceCategory? category,
        string? nationality,
        RegisterTarget target,
        bool hasImmigrationDecision = false)
    {
        var suggested = Suggest(category, nationality, hasImmigrationDecision);
        return suggested == target;
    }

    private static bool IsBelgian(string? nationality)
    {
        return nationality?.Trim().Equals("Belgian", StringComparison.OrdinalIgnoreCase) == true;
    }
}