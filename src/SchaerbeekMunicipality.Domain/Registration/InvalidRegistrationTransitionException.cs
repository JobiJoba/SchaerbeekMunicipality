namespace SchaerbeekMunicipality.Domain.Registration;

public sealed class InvalidRegistrationTransitionException : Exception
{
    public InvalidRegistrationTransitionException(string message)
        : base(message)
    {
    }
}