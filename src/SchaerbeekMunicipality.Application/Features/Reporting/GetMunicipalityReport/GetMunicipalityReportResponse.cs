namespace SchaerbeekMunicipality.Application.Features.Reporting.GetMunicipalityReport;

public sealed record MunicipalityReportSummary(
    int RegistrationsCompleted,
    int BirthDeclarationsConfirmed,
    int AddressChangesConfirmed,
    double? AveragePoliceWaitDays,
    double? AverageIntakeToRegisteredDays,
    double RejectionRatePercent);

public sealed record MonthlyVolumePoint(
    string MonthLabel,
    int Registrations,
    int BirthDeclarations,
    int AddressChanges);

public sealed record BacklogStatusRow(
    string Workflow,
    string Status,
    int Count);

public sealed record OutcomeBreakdown(
    int Registered,
    int Rejected,
    int Suspended);

public sealed record GetMunicipalityReportResponse(
    int Months,
    MunicipalityReportSummary Summary,
    IReadOnlyList<MonthlyVolumePoint> VolumeSeries,
    IReadOnlyList<BacklogStatusRow> Backlog,
    OutcomeBreakdown Outcomes);
