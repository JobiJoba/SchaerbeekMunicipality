namespace SchaerbeekMunicipality.Domain.IdentityDocuments;

public sealed class InvalidDocumentRequestTransitionException(string message) : Exception(message);
