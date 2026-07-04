namespace SchaerbeekMunicipality.Domain.Household;

public readonly record struct HouseholdMemberId(Guid Value)
{
    public static HouseholdMemberId New() => new(Guid.NewGuid());

    public static HouseholdMemberId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
