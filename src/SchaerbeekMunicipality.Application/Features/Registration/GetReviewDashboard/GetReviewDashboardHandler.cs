using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Common;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.GetReviewDashboard;

public sealed class GetReviewDashboardHandler(
    IRegistrationCaseRepository caseRepository,
    IBirthDeclarationCaseRepository birthDeclarationCaseRepository,
    RegistrationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer)
{
    public async Task<GetReviewDashboardResponse> Handle(CancellationToken cancellationToken)
    {
        authorization.EnsureCanViewReviewDashboard(currentOfficer);

        var registrationCases = await caseRepository.ListAsync(cancellationToken);
        var birthCases = await birthDeclarationCaseRepository.ListAsync(cancellationToken);
        var officerId = OfficerId.From(currentOfficer.OfficerId);

        var openCases = registrationCases.Count(c =>
            c.IsLockedTo(officerId) &&
            c.Status is RegistrationCaseStatus.Intake or RegistrationCaseStatus.UnderReview);

        var unassignedRegistration = registrationCases.Count(IsUnassignedRegistration);

        var awaitingPolice = registrationCases.Count(c =>
            c.Status == RegistrationCaseStatus.AwaitingPoliceVerification);

        var readyForDecision = registrationCases.Count(c => c.IsReadyForApproval);

        var suspendedRegistration = registrationCases.Count(c =>
            c.Status == RegistrationCaseStatus.Suspended);

        var birthUnassigned = birthCases.Count(IsUnassignedBirth);

        var readyForConfirmation = birthCases.Count(c => c.IsReadyForConfirmation);

        var statistics = new List<ReviewDashboardStatistic>
        {
            new("My open cases", openCases, "/registration/cases?filter=mine", false),
            new("Unassigned", unassignedRegistration, "/registration/cases?filter=unassigned",
                unassignedRegistration > 0),
            new("Awaiting police", awaitingPolice, "/registration/police-verifications", false),
            new("Ready for decision", readyForDecision, "/registration/cases", true),
            new("Suspended", suspendedRegistration, "/registration/cases", false),
            new("Birth unassigned", birthUnassigned, "/birth-declarations?filter=unassigned", birthUnassigned > 0),
            new("Ready for confirmation", readyForConfirmation, "/birth-declarations?filter=ready",
                readyForConfirmation > 0)
        };

        var actionable = registrationCases
            .Where(IsActionableRegistration)
            .Select(c => new ActionableCandidate(
                new ActionableCaseRow(
                    c.Id.Value,
                    ReviewDashboardCaseType.Registration,
                    c.Status.ToDisplayString(),
                    c.VisitReason.ToDisplayString(),
                    c.OpenedAt,
                    BuildRegistrationSummary(c)),
                c.IsReadyForApproval,
                IsUnassignedRegistration(c),
                c.Status == RegistrationCaseStatus.Suspended,
                c.Status == RegistrationCaseStatus.AwaitingPoliceVerification))
            .Concat(birthCases
                .Where(IsActionableBirth)
                .Select(c => new ActionableCandidate(
                    new ActionableCaseRow(
                        c.Id.Value,
                        ReviewDashboardCaseType.BirthDeclaration,
                        c.Status.ToDisplayString(),
                        "Birth declaration",
                        c.OpenedAt,
                        BuildBirthSummary(c)),
                    c.IsReadyForConfirmation,
                    IsUnassignedBirth(c),
                    c.Status == BirthDeclarationCaseStatus.Suspended,
                    false)))
            .OrderByDescending(c => c.IsReady)
            .ThenByDescending(c => c.IsUnassigned)
            .ThenByDescending(c => c.IsSuspended)
            .ThenByDescending(c => c.IsAwaitingPolice)
            .ThenByDescending(c => c.Row.OpenedAt)
            .Take(10)
            .Select(c => c.Row)
            .ToList();

        return new GetReviewDashboardResponse(statistics, actionable);
    }

    private static bool IsUnassignedRegistration(RegistrationCase registrationCase)
    {
        return registrationCase.LockedByOfficerId is null &&
               registrationCase.AssignedOfficerId is null &&
               registrationCase.Status == RegistrationCaseStatus.Intake;
    }

    private static bool IsUnassignedBirth(BirthDeclarationCase birthDeclarationCase)
    {
        return birthDeclarationCase.LockedByOfficerId is null &&
               birthDeclarationCase.AssignedOfficerId is null &&
               birthDeclarationCase.Status == BirthDeclarationCaseStatus.Intake;
    }

    private static bool IsActionableRegistration(RegistrationCase registrationCase)
    {
        return registrationCase.IsReadyForApproval ||
               registrationCase.Status == RegistrationCaseStatus.AwaitingPoliceVerification ||
               registrationCase.Status == RegistrationCaseStatus.Suspended ||
               IsUnassignedRegistration(registrationCase);
    }

    private static bool IsActionableBirth(BirthDeclarationCase birthDeclarationCase)
    {
        return birthDeclarationCase.IsReadyForConfirmation ||
               birthDeclarationCase.Status == BirthDeclarationCaseStatus.Suspended ||
               IsUnassignedBirth(birthDeclarationCase);
    }

    private static string BuildRegistrationSummary(RegistrationCase registrationCase)
    {
        return registrationCase.Status switch
        {
            RegistrationCaseStatus.AwaitingPoliceVerification => "Awaiting police residence check",
            RegistrationCaseStatus.Suspended => $"Suspended — {registrationCase.SuspensionReason}",
            _ when registrationCase.IsReadyForApproval => "Ready for officer decision",
            _ when IsUnassignedRegistration(registrationCase) => "Unassigned — awaiting intake",
            _ => registrationCase.Status.ToDisplayString()
        };
    }

    private static string BuildBirthSummary(BirthDeclarationCase birthDeclarationCase)
    {
        return birthDeclarationCase.Status switch
        {
            BirthDeclarationCaseStatus.Suspended => $"Suspended — {birthDeclarationCase.SuspensionReason}",
            _ when birthDeclarationCase.IsReadyForConfirmation => "Ready for confirmation",
            _ when IsUnassignedBirth(birthDeclarationCase) => "Unassigned — awaiting intake",
            _ => birthDeclarationCase.Status.ToDisplayString()
        };
    }

    private sealed record ActionableCandidate(
        ActionableCaseRow Row,
        bool IsReady,
        bool IsUnassigned,
        bool IsSuspended,
        bool IsAwaitingPolice);
}