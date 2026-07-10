namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress.RequestPoliceVerification;

public sealed record RequestPoliceVerificationResponse(
    Guid CaseId,
    Guid RequestId,
    int AttemptNumber,
    string Status);
