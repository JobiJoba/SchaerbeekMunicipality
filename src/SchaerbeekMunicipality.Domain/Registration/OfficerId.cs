namespace SchaerbeekMunicipality.Domain.Registration;

public readonly record struct OfficerId(Guid Value)
{
    public static OfficerId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
