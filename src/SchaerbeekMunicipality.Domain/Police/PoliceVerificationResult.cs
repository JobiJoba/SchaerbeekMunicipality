namespace SchaerbeekMunicipality.Domain.Police;

public enum PoliceVerificationResult
{
    Confirmed = 0,
    NotFound = 1,
    AddressIncorrect = 2,
    MailboxOnly = 3,
    EmptyDwelling = 4,
    RefusedAccess = 5,
    Incomplete = 6,
}
