using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public sealed class ChangeOfAddressHouseholdMemberLink
{
    private ChangeOfAddressHouseholdMemberLink()
    {
    }

    public PersonId PersonId { get; private set; }

    public static ChangeOfAddressHouseholdMemberLink Create(PersonId personId) =>
        new() { PersonId = personId };
}
