namespace SchaerbeekMunicipality.Domain.Documents;

public readonly record struct AdministrativeDocumentId(Guid Value)
{
    public static AdministrativeDocumentId New()
    {
        return new AdministrativeDocumentId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}