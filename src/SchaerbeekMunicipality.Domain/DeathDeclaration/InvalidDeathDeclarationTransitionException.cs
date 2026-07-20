namespace SchaerbeekMunicipality.Domain.DeathDeclaration;

public sealed class InvalidDeathDeclarationTransitionException(string message) : Exception(message);
