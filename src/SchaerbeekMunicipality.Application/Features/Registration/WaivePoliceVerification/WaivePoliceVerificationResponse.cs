namespace SchaerbeekMunicipality.Application.Features.Registration.WaivePoliceVerification;

public sealed record WaivePoliceVerificationResponse(
    Guid CaseId,
    string Status,
    bool AddressConfirmed);
