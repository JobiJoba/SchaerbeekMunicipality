namespace SchaerbeekMunicipality.Domain.Police;

public readonly record struct PoliceVerificationRequestId(Guid Value)
{
    public static PoliceVerificationRequestId New() => new(Guid.NewGuid());

    public static PoliceVerificationRequestId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
