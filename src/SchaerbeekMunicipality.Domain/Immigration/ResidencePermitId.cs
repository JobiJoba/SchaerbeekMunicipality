namespace SchaerbeekMunicipality.Domain.Immigration;

public readonly record struct ResidencePermitId(Guid Value)
{
    public static ResidencePermitId New()
    {
        return new ResidencePermitId(Guid.NewGuid());
    }

    public static ResidencePermitId From(Guid value)
    {
        return new ResidencePermitId(value);
    }
}