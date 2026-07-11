namespace SchaerbeekMunicipality.Web.DesignSystem.Components.Data;

public sealed record AppChecklistItem(
    string Question,
    bool IsSatisfied,
    string? BlockingReason = null);
