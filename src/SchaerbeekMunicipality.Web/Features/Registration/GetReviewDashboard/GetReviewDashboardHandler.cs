using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetReviewDashboard;

public sealed class GetReviewDashboardHandler(
    IRegistrationCaseRepository caseRepository,
    RegistrationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<GetReviewDashboardResponse> Handle(CancellationToken cancellationToken)
    {
        authorization.EnsureCanViewReviewDashboard(currentOfficer);

        var cases = await caseRepository.ListAsync(cancellationToken);
        var officerId = OfficerId.From(currentOfficer.OfficerId);

        var openCases = cases.Count(c =>
            c.IsLockedTo(officerId) &&
            c.Status is RegistrationCaseStatus.Intake or RegistrationCaseStatus.UnderReview);

        var unassigned = cases.Count(IsUnassigned);

        var awaitingPolice = cases.Count(c =>
            c.Status == RegistrationCaseStatus.AwaitingPoliceVerification);

        var readyForDecision = cases.Count(c => c.IsReadyForApproval);

        var suspended = cases.Count(c => c.Status == RegistrationCaseStatus.Suspended);

        var statistics = new List<ReviewDashboardStatistic>
        {
            new("My open cases", openCases, "/registration/cases?filter=mine", false),
            new("Unassigned", unassigned, "/registration/cases?filter=unassigned", unassigned > 0),
            new("Awaiting police", awaitingPolice, "/registration/police-verifications", false),
            new("Ready for decision", readyForDecision, "/registration/cases", true),
            new("Suspended", suspended, "/registration/cases", false),
        };

        var actionable = cases
            .Where(IsActionable)
            .OrderByDescending(c => c.IsReadyForApproval)
            .ThenByDescending(IsUnassigned)
            .ThenByDescending(c => c.Status == RegistrationCaseStatus.Suspended)
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

    private static bool IsUnassigned(RegistrationCase registrationCase) =>
        registrationCase.LockedByOfficerId is null &&
        registrationCase.AssignedOfficerId is null &&
        registrationCase.Status == RegistrationCaseStatus.Intake;

    private static bool IsActionable(RegistrationCase registrationCase) =>
        registrationCase.IsReadyForApproval ||
        registrationCase.Status == RegistrationCaseStatus.AwaitingPoliceVerification ||
        registrationCase.Status == RegistrationCaseStatus.Suspended ||
        IsUnassigned(registrationCase);

    private static string BuildSummary(RegistrationCase registrationCase) =>
        registrationCase.Status switch
        {
            RegistrationCaseStatus.AwaitingPoliceVerification => "Awaiting police residence check",
            RegistrationCaseStatus.Suspended => $"Suspended — {registrationCase.SuspensionReason}",
            _ when registrationCase.IsReadyForApproval => "Ready for officer decision",
            _ when IsUnassigned(registrationCase) => "Unassigned — awaiting intake",
            _ => registrationCase.Status.ToString(),
        };
}
