using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetReviewDashboard;

public sealed class GetReviewDashboardHandler(
    IRegistrationCaseRepository caseRepository,
    ICurrentOfficer currentOfficer)
{
    public async Task<GetReviewDashboardResponse> Handle(CancellationToken cancellationToken)
    {
        var cases = await caseRepository.ListAsync(cancellationToken);
        var officerId = currentOfficer.OfficerId;

        var openCases = cases.Count(c =>
            c.AssignedOfficerId.Value == officerId &&
            c.Status is RegistrationCaseStatus.Intake or RegistrationCaseStatus.UnderReview);

        var awaitingPolice = cases.Count(c =>
            c.Status == RegistrationCaseStatus.AwaitingPoliceVerification);

        var readyForDecision = cases.Count(c => c.IsReadyForApproval);

        var suspended = cases.Count(c => c.Status == RegistrationCaseStatus.Suspended);

        var statistics = new List<ReviewDashboardStatistic>
        {
            new("My open cases", openCases, "/registration/cases", false),
            new("Awaiting police", awaitingPolice, "/registration/police-verifications", false),
            new("Ready for decision", readyForDecision, "/registration/cases", true),
            new("Suspended", suspended, "/registration/cases", false),
        };

        var actionable = cases
            .Where(c =>
                c.IsReadyForApproval ||
                c.Status == RegistrationCaseStatus.AwaitingPoliceVerification ||
                c.Status == RegistrationCaseStatus.Suspended)
            .OrderByDescending(c => c.IsReadyForApproval)
            .ThenByDescending(c => c.OpenedAt)
            .Take(10)
            .Select(c => new ActionableCaseRow(
                c.Id.Value,
                c.Status.ToString(),
                c.VisitReason.ToString(),
                c.OpenedAt,
                BuildSummary(c)))
            .ToList();

        return new GetReviewDashboardResponse(statistics, actionable);
    }

    private static string BuildSummary(RegistrationCase registrationCase) =>
        registrationCase.Status switch
        {
            RegistrationCaseStatus.AwaitingPoliceVerification => "Awaiting police residence check",
            RegistrationCaseStatus.Suspended => $"Suspended — {registrationCase.SuspensionReason}",
            _ when registrationCase.IsReadyForApproval => "Ready for officer decision",
            _ => registrationCase.Status.ToString(),
        };
}
