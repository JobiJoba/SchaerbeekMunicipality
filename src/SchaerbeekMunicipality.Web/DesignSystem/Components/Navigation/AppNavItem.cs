namespace SchaerbeekMunicipality.Web.DesignSystem.Components.Navigation;

public sealed record AppNavItem(
    string Label,
    string Href,
    string Icon,
    string? RequiredRole = null,
    int? BadgeCount = null);