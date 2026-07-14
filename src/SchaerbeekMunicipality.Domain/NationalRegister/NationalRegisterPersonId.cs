namespace SchaerbeekMunicipality.Domain.NationalRegister;

public readonly record struct NationalRegisterPersonId(Guid Value)
{
    public static NationalRegisterPersonId New()
    {
        return new NationalRegisterPersonId(Guid.NewGuid());
    }

    public static NationalRegisterPersonId From(Guid value)
    {
        return new NationalRegisterPersonId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}