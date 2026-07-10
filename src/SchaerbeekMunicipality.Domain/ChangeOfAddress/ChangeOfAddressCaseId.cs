namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public readonly record struct ChangeOfAddressCaseId(Guid Value)
{
    public static ChangeOfAddressCaseId New() => new(Guid.NewGuid());

    public static ChangeOfAddressCaseId From(Guid value) => new(value);
}
