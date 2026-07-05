namespace SchaerbeekMunicipality.Web.Features.Registration.GetReviewDashboard;

public sealed record ReviewDashboardStatistic(
    string Label,
    int Value,
    string Href,
    bool Accent);

public sealed record ActionableCaseRow(
    Guid CaseId,
    string Status,
    string VisitReason,
    DateTimeOffset OpenedAt,
    string Summary);

public sealed record GetReviewDashboardResponse(
    IReadOnlyList<ReviewDashboardStatistic> Statistics,
    IReadOnlyList<ActionableCaseRow> ActionableCases);
