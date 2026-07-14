namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public readonly record struct ChangeOfAddressCaseId(Guid Value)
{
    public static ChangeOfAddressCaseId New()
    {
        return new ChangeOfAddressCaseId(Guid.NewGuid());
    }

    public static ChangeOfAddressCaseId From(Guid value)
    {
        return new ChangeOfAddressCaseId(value);
    }
}