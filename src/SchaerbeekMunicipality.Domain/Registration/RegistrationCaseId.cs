namespace SchaerbeekMunicipality.Domain.Registration;

public readonly record struct RegistrationCaseId(Guid Value)
{
    public static RegistrationCaseId New()
    {
        return new RegistrationCaseId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}