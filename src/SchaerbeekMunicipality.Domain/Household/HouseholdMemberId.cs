namespace SchaerbeekMunicipality.Domain.Household;

public readonly record struct HouseholdMemberId(Guid Value)
{
    public static HouseholdMemberId New()
    {
        return new HouseholdMemberId(Guid.NewGuid());
    }

    public static HouseholdMemberId From(Guid value)
    {
        return new HouseholdMemberId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}