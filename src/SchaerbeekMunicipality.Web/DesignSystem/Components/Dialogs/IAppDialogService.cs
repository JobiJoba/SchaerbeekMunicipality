namespace SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;

public interface IAppDialogService
{
    Task<bool> ConfirmAsync(
        string title,
        string message,
        string confirmText,
        bool destructive = false,
        string? consequence = null,
        CancellationToken cancellationToken = default);
}
