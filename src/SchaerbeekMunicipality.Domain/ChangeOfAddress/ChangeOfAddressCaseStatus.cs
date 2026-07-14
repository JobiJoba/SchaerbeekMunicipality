namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public enum ChangeOfAddressCaseStatus
{
    Intake = 0,
    AwaitingPoliceVerification = 1,
    UnderReview = 2,
    Confirmed = 3,
    Rejected = 4
}