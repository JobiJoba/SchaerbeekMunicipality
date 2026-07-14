namespace SchaerbeekMunicipality.Domain.Police;

public sealed class InvalidPoliceVerificationException(string message) : Exception(message);