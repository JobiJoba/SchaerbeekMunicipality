namespace SchaerbeekMunicipality.Application.Features.Registration.GetReviewDashboard;

public enum ReviewDashboardCaseType
{
    Registration,
    BirthDeclaration,
    RegisterAmendment,
    DeathDeclaration
}

public sealed record ReviewDashboardStatistic(
    string Label,
    int Value,
    string Href,
    bool Accent);

public sealed record ActionableCaseRow(
    Guid CaseId,
    ReviewDashboardCaseType CaseType,
    string Status,
    string Procedure,
    DateTimeOffset OpenedAt,
    string Summary);

public sealed record GetReviewDashboardResponse(
    IReadOnlyList<ReviewDashboardStatistic> Statistics,
    IReadOnlyList<ActionableCaseRow> ActionableCases);