namespace SchaerbeekMunicipality.Domain.IdentityDocuments;

public enum DocumentRequestCaseStatus
{
    Submitted = 0,
    InProduction = 1,
    ReadyForCollection = 2,
    Issued = 3,
    Cancelled = 4
}