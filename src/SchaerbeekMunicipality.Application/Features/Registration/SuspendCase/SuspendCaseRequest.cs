using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.SuspendCase;

public sealed record SuspendCaseRequest(SuspensionReason Reason, string? Notes);

public sealed record SuspendCaseResponse(
    Guid CaseId,
    string Status,
    string Reason);