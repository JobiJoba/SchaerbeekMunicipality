using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.SuspendCase;

public sealed record SuspendCaseRequest(SuspensionReason Reason, string? Notes);

public sealed record SuspendCaseResponse(
    Guid CaseId,
    string Status,
    string Reason);
