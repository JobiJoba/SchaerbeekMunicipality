namespace SchaerbeekMunicipality.Domain.RegisterAmendment;

public readonly record struct RegisterAmendmentCaseId(Guid Value)
{
    public static RegisterAmendmentCaseId New()
    {
        return new RegisterAmendmentCaseId(Guid.NewGuid());
    }

    public static RegisterAmendmentCaseId From(Guid value)
    {
        return new RegisterAmendmentCaseId(value);
    }
}
