namespace SchaerbeekMunicipality.Domain.Registration;

public readonly record struct CaseAuditEntryId(Guid Value)
{
    public static CaseAuditEntryId New()
    {
        return new CaseAuditEntryId(Guid.NewGuid());
    }

    public static CaseAuditEntryId From(Guid value)
    {
        return new CaseAuditEntryId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}