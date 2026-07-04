using MudBlazor;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.DesignSystem.Tokens;

namespace SchaerbeekMunicipality.Web.DesignSystem.Components.Data;

public static class AppStatusChipMappings
{
    public static (string Label, string Icon, string Background, string Foreground) ForRegistrationCaseStatus(
        RegistrationCaseStatus status) =>
        status switch
        {
            RegistrationCaseStatus.Intake => ("Intake", Icons.Material.Outlined.Inbox, SchaerbeekColors.InfoBackground, SchaerbeekColors.Info),
            RegistrationCaseStatus.AwaitingPoliceVerification => ("Awaiting police", Icons.Material.Outlined.LocalPolice, SchaerbeekColors.WarningBackground, SchaerbeekColors.Warning),
            RegistrationCaseStatus.UnderReview => ("Under review", Icons.Material.Outlined.RateReview, SchaerbeekColors.InfoBackground, SchaerbeekColors.Info),
            RegistrationCaseStatus.Approved => ("Approved", Icons.Material.Outlined.CheckCircle, SchaerbeekColors.SuccessBackground, SchaerbeekColors.Success),
            RegistrationCaseStatus.Registered => ("Registered", Icons.Material.Outlined.Verified, SchaerbeekColors.SuccessBackground, SchaerbeekColors.Success),
            RegistrationCaseStatus.Rejected => ("Rejected", Icons.Material.Outlined.Cancel, SchaerbeekColors.ErrorBackground, SchaerbeekColors.Error),
            RegistrationCaseStatus.Suspended => ("Suspended", Icons.Material.Outlined.PauseCircle, SchaerbeekColors.SurfaceSunken, SchaerbeekColors.TextSecondary),
            _ => (status.ToString(), Icons.Material.Outlined.Info, SchaerbeekColors.SurfaceSunken, SchaerbeekColors.TextSecondary),
        };

    public static (string Label, string Icon, string Background, string Foreground) ForSeverity(AppSeverity severity) =>
        severity switch
        {
            AppSeverity.Info => ("Info", Icons.Material.Outlined.Info, SchaerbeekColors.InfoBackground, SchaerbeekColors.Info),
            AppSeverity.Success => ("Success", Icons.Material.Outlined.CheckCircle, SchaerbeekColors.SuccessBackground, SchaerbeekColors.Success),
            AppSeverity.Warning => ("Warning", Icons.Material.Outlined.Warning, SchaerbeekColors.WarningBackground, SchaerbeekColors.Warning),
            AppSeverity.Error => ("Error", Icons.Material.Outlined.Error, SchaerbeekColors.ErrorBackground, SchaerbeekColors.Error),
            _ => ("", Icons.Material.Outlined.Circle, SchaerbeekColors.SurfaceSunken, SchaerbeekColors.TextSecondary),
        };
}
