namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public enum BirthDeclarationRejectionReason
{
    MissingMedicalDeclaration = 0,
    DeclarationWindowExpired = 1,
    ParentNotRegistered = 2,
    Other = 3,
}
