namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public readonly record struct BirthDeclarationCaseId(Guid Value)
{
    public static BirthDeclarationCaseId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
