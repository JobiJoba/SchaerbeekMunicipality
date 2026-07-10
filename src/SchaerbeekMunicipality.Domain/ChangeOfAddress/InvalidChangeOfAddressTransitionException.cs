namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public sealed class InvalidChangeOfAddressTransitionException(string message) : Exception(message);
