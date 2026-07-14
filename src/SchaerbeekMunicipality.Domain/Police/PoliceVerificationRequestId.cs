namespace SchaerbeekMunicipality.Domain.Police;

public readonly record struct PoliceVerificationRequestId(Guid Value)
{
    public static PoliceVerificationRequestId New()
    {
        return new PoliceVerificationRequestId(Guid.NewGuid());
    }

    public static PoliceVerificationRequestId From(Guid value)
    {
        return new PoliceVerificationRequestId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}