using Wpf.Ui.Controls;

namespace BTB.Core.Dialogs;

/// <summary>
/// Provides methods for displaying dialogs to the user.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Displays an informational dialog.
    /// </summary>
    Task ShowInfo(object content, string? title = null, DialogOptions? options = null);

    /// <summary>
    /// Displays an error dialog.
    /// </summary>
    Task ShowError(object content, string? title = null, DialogOptions? options = null);

    /// <summary>
    /// Displays a confirmation dialog and indicates whether the user confirmed the action.
    /// </summary>
    Task<bool> Confirm(object content, string? title = null, DialogOptions? options = null);

    /// <summary>
    /// Displays a dialog and provides the user's response.
    /// </summary>
    Task<ContentDialogResult> Show(object content, string? title = null, DialogOptions? options = null);
}
