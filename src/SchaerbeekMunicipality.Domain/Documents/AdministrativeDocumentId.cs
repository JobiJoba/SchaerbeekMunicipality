namespace SchaerbeekMunicipality.Domain.Documents;

public readonly record struct AdministrativeDocumentId(Guid Value)
{
    public static AdministrativeDocumentId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
