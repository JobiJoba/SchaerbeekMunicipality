namespace SchaerbeekMunicipality.Domain.Registration;

public sealed class RegistrationCase
{
    private RegistrationCase()
    {
    }

    public RegistrationCaseId Id { get; private set; }

    public RegistrationCaseStatus Status { get; private set; }

    public static RegistrationCase Create()
    {
        return new RegistrationCase
        {
            Id = RegistrationCaseId.New(),
            Status = RegistrationCaseStatus.Intake,
        };
    }
}
