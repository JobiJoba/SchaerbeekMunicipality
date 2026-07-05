namespace SchaerbeekMunicipality.Domain.Registration;

public enum RejectionReason
{
    InsufficientIdentityEvidence = 0,
    NoLegalResidenceBasis = 1,
    AddressNotGenuine = 2,
    DuplicateIdentity = 3,
    IllegalStay = 4,
    Other = 5,
}
