using SchaerbeekMunicipality.Domain.Police;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordPoliceResult;

public sealed record RecordPoliceResultResponse(
    Guid RequestId,
    Guid CaseId,
    PoliceVerificationResult Result,
    bool AddressConfirmed,
    string Status,
    string CaseType = "Registration");