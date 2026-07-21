namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public enum ChangeOfAddressRejectionReason
{
    InvalidAddress = 0,
    NegativePoliceVerification = 1,
    MissingDocumentation = 2,
    Other = 3,
    OpenedInError = 4
}