using MudBlazor;

namespace SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;

public static class AppDialogOptions
{
    public static DialogOptions Small => new()
    {
        MaxWidth = MaxWidth.ExtraSmall,
        FullWidth = true,
        CloseButton = true,
        BackdropClick = false,
    };

    public static DialogOptions Medium => new()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        BackdropClick = false,
    };

    public static DialogOptions Large => new()
    {
        MaxWidth = MaxWidth.Medium,
        FullWidth = true,
        CloseButton = true,
        BackdropClick = false,
    };
}
