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
        options ??= CreateOptions(null, _localization["Common.Ok"]);
        return Show(content, title ?? _localization["Dialog.Info"], options);
    }

    /// <summary>
    /// Shows an error dialog with the specified content and title.
    /// </summary>
    public Task ShowError(object content, string? title = null, DialogOptions? options = null)
    {
        options ??= CreateOptions(null, _localization["Common.Ok"]);
        return Show(content, title ?? _localization["Dialog.Error"], options);
    }

    /// <summary>
    /// Shows a confirmation dialog and indicates whether the primary action was selected.
    /// </summary>
    public async Task<bool> Confirm(object content, string? title = null, DialogOptions? options = null)
    {
        options ??= CreateOptions(_localization["Common.Yes"], _localization["Common.No"]);
        ContentDialogResult result = await Show(content, title ?? _localization["Dialog.Confirmation"], options);
        return result == ContentDialogResult.Primary;
    }

    /// <summary>
    /// Shows a dialog with the specified content, title, and options.
    /// </summary>
    public async Task<ContentDialogResult> Show(object content, string? title = null, DialogOptions? options = null)
    {
        options ??= CreateOptions(_localization["Common.Apply"], _localization["Common.Close"]);

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
            }
        };

        if (options.StyleKey != null)
        {
            if (Application.Current.TryFindResource(options.StyleKey) is Style style)
            {
                dialog.Style = style;
            }
            else
            {
                throw new InvalidOperationException($"Dialog style '{options.StyleKey}' not found.");
            }
        }

        if (!string.IsNullOrWhiteSpace(options.PrimaryButtonText))
        {
            dialog.PrimaryButtonText = options.PrimaryButtonText;
        }

        if (!string.IsNullOrWhiteSpace(options.SecondaryButtonText))
        {
            dialog.SecondaryButtonText = options.SecondaryButtonText;
        }

        if (!string.IsNullOrWhiteSpace(options.CloseButtonText))
        {
            dialog.CloseButtonText = options.CloseButtonText;
        }

        if (options.PrimaryButtonValidation != null)
        {
            bool allowClose = false;

            dialog.ButtonClicked += async (sender, args) =>
            {
                if (args.Button != ContentDialogButton.Primary)
                {
                    return;
                }

                allowClose = await options.PrimaryButtonValidation();

                if (allowClose)
                {
                    dialog.Hide();
                }
            };

            dialog.Closing += (sender, args) =>
            {
                if (args.Result == ContentDialogResult.Primary && !allowClose)
                {
                    args.Cancel = true;
                }
            };
        }

        return dialog;
    }

    /// <summary>
    /// Creates dialog options by applying the specified dimensions and button texts.
    /// </summary>
    private DialogOptions CreateOptions(string? primaryButtonText = null, string? closeButtonText = null)
    {
        DialogOptions dialogOptions = new DialogOptions
        {
            CloseButtonText = _localization["Common.Ok"]
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