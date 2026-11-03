using System.Windows;
using System.Windows.Controls;

using BTB.Core.Localization;

using Wpf.Ui.Controls;

namespace BTB.Core.Dialogs;

/// <summary>
/// Provides methods for displaying localized content dialogs.
/// </summary>
public sealed class DialogService : IDialogService
{
    #region Private Fields
    private readonly ILocalizationService _localization;
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public DialogService(ILocalizationService localization)
    {
        _localization = localization;
    }

    #region Public Methods

    /// <summary>
    /// Shows an information dialog with the specified content and title.
    /// </summary>
    public Task ShowInfo(object content, string? title = null, DialogOptions? options = null)
    {
        options ??= CreateOptions(options, null, "Common.Ok");
        return Show(content, title ?? _localization["Dialog.Info"], options);
    }

    /// <summary>
    /// Shows an error dialog with the specified content and title.
    /// </summary>
    public Task ShowError(object content, string? title = null, DialogOptions? options = null)
    {
        options ??= CreateOptions(options, null, "Common.Ok");
        return Show(content, title ?? _localization["Dialog.Error"], options);
    }

    /// <summary>
    /// Shows a confirmation dialog and indicates whether the primary action was selected.
    /// </summary>
    public async Task<bool> Confirm(object content, string? title = null, DialogOptions? options = null)
    {
        options ??= CreateOptions(options, "Common.Yes", "Common.No");
        ContentDialogResult result = await Show(content, title ?? _localization["Dialog.Confirmation"], options);
        return result == ContentDialogResult.Primary;
    }

    /// <summary>
    /// Shows a dialog with the specified content, title, and options.
    /// </summary>
    public async Task<ContentDialogResult> Show(object content, string? title = null, DialogOptions? options = null)
    {
        options ??= CreateOptions(options, "Common.Apply", "Common.Close");

        ContentDialog dialog = CreateDialog(content, title, options);

        ContentDialogResult result = await dialog.ShowAsync();

        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a content dialog for the application's main window.
    /// </summary>
    private ContentDialog CreateDialog(object content, string? title, DialogOptions options)
    {
        Window window = Application.Current.MainWindow ?? throw new InvalidOperationException("There is no active main window.");

        ContentDialogHost host = ContentDialogHost.GetForWindow(window) ?? throw new InvalidOperationException("No ContentDialogHost was found.");

        object dialogContent = content is string text
            ? new System.Windows.Controls.TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap
            }
            : content;

        ContentDialog dialog = new ContentDialog(host)
        {
            Title = title,
            Content = new ContentControl
            {
                Content = dialogContent,
                Width = options.Width,
                Height = options.Height
            },
            Style = Application.Current.TryFindResource(options.StyleKey) as Style
        };

        if (!string.IsNullOrWhiteSpace(options.PrimaryButtonText))
        {
            dialog.PrimaryButtonText = _localization[options.PrimaryButtonText];
        }

        if (!string.IsNullOrWhiteSpace(options.SecondaryButtonText))
        {
            dialog.SecondaryButtonText = _localization[options.SecondaryButtonText];
        }

        if (!string.IsNullOrWhiteSpace(options.CloseButtonText))
        {
            dialog.CloseButtonText = _localization[options.CloseButtonText];
        }

        return dialog;
    }

    /// <summary>
    /// Creates dialog options by applying the specified dimensions and button texts.
    /// </summary>
    private DialogOptions CreateOptions(DialogOptions? source, string? primaryButtonText = null, string? closeButtonText = null)
    {
        DialogOptions dialogOptions = new DialogOptions
        {
            Width = source?.Width ?? 400,
            Height = source?.Height ?? 250,
            CloseButtonText = "Common.Ok"
        };

        if (!string.IsNullOrWhiteSpace(primaryButtonText))
        {
            dialogOptions.PrimaryButtonText = primaryButtonText;
        }

        if (!string.IsNullOrWhiteSpace(closeButtonText))
        {
            dialogOptions.CloseButtonText = closeButtonText;
        }

        return dialogOptions;
    }

    #endregion
}