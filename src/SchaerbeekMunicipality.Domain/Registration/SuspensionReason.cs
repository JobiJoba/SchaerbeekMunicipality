namespace SchaerbeekMunicipality.Domain.Registration;

public enum SuspensionReason
{
    MissingDocuments = 0,
    AwaitingFederalDecision = 1,
    MarriageRecognitionPending = 2,
    InvestigationRequired = 3,
    Other = 4
}