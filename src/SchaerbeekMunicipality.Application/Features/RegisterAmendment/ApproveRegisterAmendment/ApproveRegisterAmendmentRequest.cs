namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.ApproveRegisterAmendment;

public sealed record ApproveRegisterAmendmentRequest(string? Notes);

public sealed record ApproveRegisterAmendmentResponse(
    Guid CaseId,
    string Status,
    DateTimeOffset ApprovedAt);
