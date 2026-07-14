namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public readonly record struct BirthDeclarationCaseId(Guid Value)
{
    public static BirthDeclarationCaseId New()
    {
        return new BirthDeclarationCaseId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}