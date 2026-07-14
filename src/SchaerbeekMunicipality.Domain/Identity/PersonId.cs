namespace SchaerbeekMunicipality.Domain.Identity;

public readonly record struct PersonId(Guid Value)
{
    public static PersonId New()
    {
        return new PersonId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}