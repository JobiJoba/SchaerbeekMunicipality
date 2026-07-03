namespace SchaerbeekMunicipality.Domain.Registration;

public readonly record struct RegistrationCaseId(Guid Value)
{
    public static RegistrationCaseId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
