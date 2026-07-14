namespace SchaerbeekMunicipality.Application.Features.Registration.ListPendingPoliceVerifications;

public sealed record PendingPoliceVerificationDto(
    Guid RequestId,
    Guid CaseId,
    string CaseType,
    string PersonName,
    string Address,
    DateTimeOffset RequestedAt,
    int AttemptNumber);

public sealed record ListPendingPoliceVerificationsResponse(
    IReadOnlyList<PendingPoliceVerificationDto> Items,
    int TotalCount);