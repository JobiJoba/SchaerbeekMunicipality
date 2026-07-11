namespace SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;

public sealed record RequestPoliceVerificationResponse(
    Guid CaseId,
    Guid RequestId,
    int AttemptNumber,
    string Status);
