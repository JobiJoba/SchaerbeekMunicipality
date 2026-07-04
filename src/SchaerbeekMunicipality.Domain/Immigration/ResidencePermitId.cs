namespace SchaerbeekMunicipality.Domain.Immigration;

public readonly record struct ResidencePermitId(Guid Value)
{
    public static ResidencePermitId New() => new(Guid.NewGuid());

    public static ResidencePermitId From(Guid value) => new(value);
}
