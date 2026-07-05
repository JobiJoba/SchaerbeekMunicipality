namespace SchaerbeekMunicipality.Domain.Registration;

public readonly record struct CaseAuditEntryId(Guid Value)
{
    public static CaseAuditEntryId New() => new(Guid.NewGuid());

    public static CaseAuditEntryId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
