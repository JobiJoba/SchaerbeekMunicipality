namespace SchaerbeekMunicipality.Domain.RegisterAmendment;

public sealed class InvalidRegisterAmendmentTransitionException(string message) : Exception(message);
