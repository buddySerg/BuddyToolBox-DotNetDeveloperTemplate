namespace BTB.Core.Dialogs;

/// <summary>
/// Defines the configurable options for displaying a dialog.
/// </summary>
public sealed class DialogOptions
{
    /// <summary>
    /// Gets or sets the text displayed on the primary action button.
    /// </summary>
    public string? PrimaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the text displayed on the secondary action button.
    /// </summary>
    public string? SecondaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the text displayed on the close button.
    /// </summary>
    public string CloseButtonText { get; set; } = "Common.Close";

    /// <summary>
    /// Gets or sets the width of the dialog in device-independent units.
    /// </summary>
    public double Width { get; set; } = 600;

    /// <summary>
    /// Gets or sets the height of the dialog in device-independent units.
    /// </summary>
    public double Height { get; set; } = 500;

    /// <summary>
    /// Gets or sets the resource key of the style applied to the dialog.
    /// </summary>
    public string StyleKey { get; set; } = "CustomContentDialogStyle";
}