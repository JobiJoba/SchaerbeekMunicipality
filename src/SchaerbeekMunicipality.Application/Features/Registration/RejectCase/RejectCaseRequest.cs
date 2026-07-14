using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.RejectCase;

public sealed record RejectCaseRequest(RejectionReason Reason, string? Notes);

public sealed record RejectCaseResponse(
    Guid CaseId,
    string Status,
    string Reason);