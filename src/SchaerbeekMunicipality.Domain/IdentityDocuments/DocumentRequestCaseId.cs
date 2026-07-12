namespace SchaerbeekMunicipality.Domain.IdentityDocuments;

public readonly record struct DocumentRequestCaseId(Guid Value)
{
    public static DocumentRequestCaseId New() => new(Guid.NewGuid());

    public static DocumentRequestCaseId From(Guid value) => new(value);
}
