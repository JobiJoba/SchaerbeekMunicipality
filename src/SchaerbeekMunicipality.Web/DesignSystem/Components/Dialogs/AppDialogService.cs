using MudBlazor;

namespace SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;

public sealed class AppDialogService(IDialogService dialogService) : IAppDialogService
{
    public async Task<bool> ConfirmAsync(
        string title,
        string message,
        string confirmText,
        bool destructive = false,
        string? consequence = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DialogParameters<AppConfirmDialog>
        {
            { x => x.Title, title },
            { x => x.Message, message },
            { x => x.ConfirmText, confirmText },
            { x => x.Destructive, destructive },
            { x => x.Consequence, consequence },
        };

        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true,
            CloseButton = true,
            BackdropClick = false,
        };

        var dialog = await dialogService.ShowAsync<AppConfirmDialog>(title, parameters, options);
        var result = await dialog.Result;

        return result is { Canceled: false };
    }
}
