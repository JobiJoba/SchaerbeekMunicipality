namespace SchaerbeekMunicipality.Domain.IdentityDocuments;

public readonly record struct DocumentRequestCaseId(Guid Value)
{
    public static DocumentRequestCaseId New()
    {
        return new DocumentRequestCaseId(Guid.NewGuid());
    }

    public static DocumentRequestCaseId From(Guid value)
    {
        return new DocumentRequestCaseId(value);
    }
}