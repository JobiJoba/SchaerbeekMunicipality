namespace SchaerbeekMunicipality.Application.Features.Registration.GetCaseReviewChecklist;

public sealed record CaseReviewQuestionDto(
    string Key,
    string Question,
    bool IsSatisfied,
    string? BlockingReason);

public sealed record GetCaseReviewChecklistResponse(
    Guid CaseId,
    string Status,
    bool IsReadyForApproval,
    string? SuggestedRegisterTarget,
    IReadOnlyList<CaseReviewQuestionDto> Questions);
