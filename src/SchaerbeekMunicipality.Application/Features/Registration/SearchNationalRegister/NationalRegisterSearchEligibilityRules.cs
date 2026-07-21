using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

internal static class NationalRegisterSearchEligibilityRules
{
    public static bool Matches(
        NationalRegisterMatch match,
        Person? linkedPerson,
        NationalRegisterSearchEligibility eligibility)
    {
        return eligibility switch
        {
            NationalRegisterSearchEligibility.All => true,
            NationalRegisterSearchEligibility.LivingOnly => linkedPerson?.IsDeceased != true,
            NationalRegisterSearchEligibility.RegisteredResidentsOnly => IsRegisteredResident(match, linkedPerson),
            _ => true
        };
    }

    public static bool IsRegisteredResident(NationalRegisterMatch match, Person? linkedPerson)
    {
        return !string.IsNullOrWhiteSpace(match.NationalRegisterNumber)
               && linkedPerson?.NationalRegisterNumber is not null
               && linkedPerson.IsDeceased == false;
    }
}
