using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.RejectRegisterAmendment;

public sealed record RejectRegisterAmendmentRequest(
    RegisterAmendmentRejectionReason Reason,
    string? Notes);

public sealed record RejectRegisterAmendmentResponse(
    Guid CaseId,
    string Status,
    string Reason);
