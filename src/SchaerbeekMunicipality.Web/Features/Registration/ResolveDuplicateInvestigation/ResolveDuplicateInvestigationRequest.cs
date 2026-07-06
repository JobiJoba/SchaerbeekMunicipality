namespace SchaerbeekMunicipality.Web.Features.Registration.ResolveDuplicateInvestigation;

public sealed record ResolveDuplicateInvestigationRequest(string? Notes);

public sealed record ResolveDuplicateInvestigationResponse(
    Guid CaseId,
    string DuplicateInvestigationStatus,
    bool DuplicateInvestigationResolved);
