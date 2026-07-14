namespace SchaerbeekMunicipality.Domain.Household;

public readonly record struct HouseholdId(Guid Value)
{
    public static HouseholdId New()
    {
        return new HouseholdId(Guid.NewGuid());
    }

    public static HouseholdId From(Guid value)
    {
        return new HouseholdId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}