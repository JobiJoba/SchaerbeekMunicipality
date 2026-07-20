namespace SchaerbeekMunicipality.Domain.DeathDeclaration;

public enum DeathDeclarationRejectionReason
{
    MissingDeathAct = 0,
    PersonAlreadyDeceased = 1,
    InsufficientEvidence = 2,
    Other = 3
}
