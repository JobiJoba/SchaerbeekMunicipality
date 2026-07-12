using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Reporting.GetMunicipalityReport;

public sealed class GetMunicipalityReportHandler(
    IRegistrationCaseRepository registrationCaseRepository,
    IBirthDeclarationCaseRepository birthDeclarationCaseRepository,
    IChangeOfAddressCaseRepository changeOfAddressCaseRepository,
    IPoliceVerificationRepository policeVerificationRepository,
    ReportingAuthorization authorization,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    private static readonly int[] AllowedPeriods = [3, 6, 12];

    public async Task<GetMunicipalityReportResponse> Handle(
        int months,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanViewReports(currentOfficer);

        if (!AllowedPeriods.Contains(months))
        {
            throw new ArgumentOutOfRangeException(nameof(months), "Months must be 3, 6, or 12.");
        }

        var periodStart = GetPeriodStart(timeProvider.GetUtcNow(), months);

        var registrationCases = await registrationCaseRepository.ListAsync(cancellationToken);
        var birthCases = await birthDeclarationCaseRepository.ListAsync(cancellationToken);
        var changeOfAddressCases = await changeOfAddressCaseRepository.ListAsync(cancellationToken);
        var policeRequests = await policeVerificationRepository.ListAllAsync(cancellationToken);

        var completedRegistrations = registrationCases
            .Where(c => c.Status == RegistrationCaseStatus.Registered &&
                        c.ClosedAt is not null &&
                        c.ClosedAt >= periodStart)
            .ToList();

        var confirmedBirths = birthCases
            .Where(c => c.Status == BirthDeclarationCaseStatus.Confirmed &&
                        c.ClosedAt is not null &&
                        c.ClosedAt >= periodStart)
            .ToList();

        var confirmedMoves = changeOfAddressCases
            .Where(c => c.Status == ChangeOfAddressCaseStatus.Confirmed &&
                        c.ClosedAt is not null &&
                        c.ClosedAt >= periodStart)
            .ToList();

        var completedPolice = policeRequests
            .Where(r => r.CompletedAt is not null && r.CompletedAt >= periodStart)
            .ToList();

        var averagePoliceWaitDays = completedPolice.Count == 0
            ? (double?)null
            : completedPolice.Average(r => (r.CompletedAt!.Value - r.RequestedAt).TotalDays);

        var averageIntakeToRegisteredDays = completedRegistrations.Count == 0
            ? (double?)null
            : completedRegistrations.Average(c => (c.ClosedAt!.Value - c.OpenedAt).TotalDays);

        var rejectedInPeriod = CountRejectedInPeriod(registrationCases, birthCases, changeOfAddressCases, periodStart);
        var suspendedInPeriod = CountSuspendedInPeriod(registrationCases, birthCases, periodStart);
        var registeredInPeriod = completedRegistrations.Count + confirmedBirths.Count + confirmedMoves.Count;
        var terminalInPeriod = registeredInPeriod + rejectedInPeriod + suspendedInPeriod;
        var rejectionRatePercent = terminalInPeriod == 0
            ? 0
            : Math.Round(rejectedInPeriod * 100d / terminalInPeriod, 1);

        var summary = new MunicipalityReportSummary(
            completedRegistrations.Count,
            confirmedBirths.Count,
            confirmedMoves.Count,
            averagePoliceWaitDays is null ? null : Math.Round(averagePoliceWaitDays.Value, 1),
            averageIntakeToRegisteredDays is null ? null : Math.Round(averageIntakeToRegisteredDays.Value, 1),
            rejectionRatePercent);

        var volumeSeries = BuildVolumeSeries(
            periodStart,
            months,
            completedRegistrations,
            confirmedBirths,
            confirmedMoves);

        var backlog = BuildBacklog(registrationCases, birthCases, changeOfAddressCases);

        var outcomes = new OutcomeBreakdown(
            registeredInPeriod,
            rejectedInPeriod,
            suspendedInPeriod);

        return new GetMunicipalityReportResponse(
            months,
            summary,
            volumeSeries,
            backlog,
            outcomes);
    }

    private static DateTimeOffset GetPeriodStart(DateTimeOffset now, int months)
    {
        var start = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero)
            .AddMonths(-(months - 1));
        return start;
    }

    private static int CountRejectedInPeriod(
        IReadOnlyList<RegistrationCase> registrationCases,
        IReadOnlyList<BirthDeclarationCase> birthCases,
        IReadOnlyList<ChangeOfAddressCase> changeOfAddressCases,
        DateTimeOffset periodStart) =>
        registrationCases.Count(c =>
            c.Status == RegistrationCaseStatus.Rejected &&
            c.ClosedAt is not null &&
            c.ClosedAt >= periodStart) +
        birthCases.Count(c =>
            c.Status == BirthDeclarationCaseStatus.Rejected &&
            c.ClosedAt is not null &&
            c.ClosedAt >= periodStart) +
        changeOfAddressCases.Count(c =>
            c.Status == ChangeOfAddressCaseStatus.Rejected &&
            c.ClosedAt is not null &&
            c.ClosedAt >= periodStart);

    private static int CountSuspendedInPeriod(
        IReadOnlyList<RegistrationCase> registrationCases,
        IReadOnlyList<BirthDeclarationCase> birthCases,
        DateTimeOffset periodStart)
    {
        return registrationCases.Count(c =>
                   c.Status == RegistrationCaseStatus.Suspended &&
                   c.OpenedAt >= periodStart) +
               birthCases.Count(c =>
                   c.Status == BirthDeclarationCaseStatus.Suspended &&
                   c.OpenedAt >= periodStart);
    }

    private static IReadOnlyList<MonthlyVolumePoint> BuildVolumeSeries(
        DateTimeOffset periodStart,
        int months,
        IReadOnlyList<RegistrationCase> completedRegistrations,
        IReadOnlyList<BirthDeclarationCase> confirmedBirths,
        IReadOnlyList<ChangeOfAddressCase> confirmedMoves)
    {
        var points = new List<MonthlyVolumePoint>(months);

        for (var i = 0; i < months; i++)
        {
            var monthStart = periodStart.AddMonths(i);
            var monthEnd = monthStart.AddMonths(1);

            points.Add(new MonthlyVolumePoint(
                monthStart.ToString("MMM yyyy"),
                completedRegistrations.Count(c => c.ClosedAt >= monthStart && c.ClosedAt < monthEnd),
                confirmedBirths.Count(c => c.ClosedAt >= monthStart && c.ClosedAt < monthEnd),
                confirmedMoves.Count(c => c.ClosedAt >= monthStart && c.ClosedAt < monthEnd)));
        }

        return points;
    }

    private static IReadOnlyList<BacklogStatusRow> BuildBacklog(
        IReadOnlyList<RegistrationCase> registrationCases,
        IReadOnlyList<BirthDeclarationCase> birthCases,
        IReadOnlyList<ChangeOfAddressCase> changeOfAddressCases)
    {
        var rows = registrationCases
            .GroupBy(c => c.Status)
            .Select(g => new BacklogStatusRow("Registration", g.Key.ToString(), g.Count()))
            .Concat(birthCases
                .GroupBy(c => c.Status)
                .Select(g => new BacklogStatusRow("Birth declaration", g.Key.ToString(), g.Count())))
            .Concat(changeOfAddressCases
                .GroupBy(c => c.Status)
                .Select(g => new BacklogStatusRow("Change of address", g.Key.ToString(), g.Count())))
            .OrderBy(r => r.Workflow)
            .ThenBy(r => r.Status)
            .ToList();

        return rows;
    }
}
