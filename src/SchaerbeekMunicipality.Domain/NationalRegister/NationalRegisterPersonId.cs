namespace SchaerbeekMunicipality.Domain.NationalRegister;

public readonly record struct NationalRegisterPersonId(Guid Value)
{
    public static NationalRegisterPersonId New() => new(Guid.NewGuid());

    public static NationalRegisterPersonId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
