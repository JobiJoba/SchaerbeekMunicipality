using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.ReferenceData;

namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public static class ChangeOfAddressRules
{
    public static bool RequiresHousingDocument(HousingSituation situation)
    {
        return situation is HousingSituation.Tenant;
    }

    public static bool IsValidMunicipalityAddress(BelgianAddress address)
    {
        return address.PostalCode == SchaerbeekCommune.PostalCode &&
               address.Municipality.Equals(SchaerbeekCommune.Name, StringComparison.OrdinalIgnoreCase);
    }
}