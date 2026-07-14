namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public sealed class InvalidBirthDeclarationTransitionException(string message) : Exception(message);