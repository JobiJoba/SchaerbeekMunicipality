namespace SchaerbeekMunicipality.Application.Features.Registration.ResolveDuplicateInvestigation;

public sealed record ResolveDuplicateInvestigationRequest(string? Notes);

public sealed record ResolveDuplicateInvestigationResponse(
    Guid CaseId,
    string DuplicateInvestigationStatus,
    bool DuplicateInvestigationResolved);