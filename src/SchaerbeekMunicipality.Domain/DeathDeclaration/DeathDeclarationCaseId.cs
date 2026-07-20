namespace SchaerbeekMunicipality.Domain.DeathDeclaration;

public readonly record struct DeathDeclarationCaseId(Guid Value)
{
    public static DeathDeclarationCaseId New()
    {
        return new DeathDeclarationCaseId(Guid.NewGuid());
    }

    public static DeathDeclarationCaseId From(Guid value)
    {
        return new DeathDeclarationCaseId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
