namespace SchaerbeekMunicipality.Domain.Registration;

public enum RegistrationCaseStatus
{
    Intake = 0,
    AwaitingPoliceVerification = 1,
    UnderReview = 2,
    Approved = 3,
    Registered = 4,
    Rejected = 5,
    Suspended = 6
}