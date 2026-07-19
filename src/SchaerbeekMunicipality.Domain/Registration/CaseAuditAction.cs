namespace SchaerbeekMunicipality.Domain.Registration;

public enum CaseAuditAction
{
    CaseOpened = 0,
    IdentityRecorded = 1,
    IdentityCorrected = 2,
    ResidenceCategorySet = 3,
    AddressDeclared = 4,
    PoliceVerificationRequested = 5,
    PoliceResultRecorded = 6,
    CaseApproved = 7,
    CaseRejected = 8,
    CaseSuspended = 9,
    CaseResumed = 10,
    RegistrationConfirmed = 11,
    CertificateIssued = 12,
    CaseAssigned = 13,
    CaseLockReleased = 14,
    DuplicateInvestigationResolved = 15,
    PoliceVerificationWaived = 16
}