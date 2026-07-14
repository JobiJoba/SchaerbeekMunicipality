namespace SchaerbeekMunicipality.Domain.Registration;

public readonly record struct OfficerId(Guid Value)
{
    public static OfficerId From(Guid value)
    {
        return new OfficerId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}