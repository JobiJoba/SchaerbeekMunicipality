namespace SchaerbeekMunicipality.Domain.Household;

public sealed class HouseholdMember
{
    private HouseholdMember()
    {
    }

    public HouseholdMemberId Id { get; private set; }

    public HouseholdId HouseholdId { get; private set; }

    public string GivenName { get; private set; } = string.Empty;

    public string FamilyName { get; private set; } = string.Empty;

    public DateOnly BirthDate { get; private set; }

    public HouseholdMemberRole Role { get; private set; }

    public static HouseholdMember Create(HouseholdMemberDetails details)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(details.GivenName);
        ArgumentException.ThrowIfNullOrWhiteSpace(details.FamilyName);

        return new HouseholdMember
        {
            Id = HouseholdMemberId.New(),
            GivenName = details.GivenName.Trim(),
            FamilyName = details.FamilyName.Trim(),
            BirthDate = details.BirthDate,
            Role = details.Role
        };
    }

    internal void AttachToHousehold(HouseholdId householdId)
    {
        HouseholdId = householdId;
    }
}