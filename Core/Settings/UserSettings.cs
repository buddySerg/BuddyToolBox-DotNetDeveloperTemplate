using System.ComponentModel.DataAnnotations;

using BTB.Core.Theme;

namespace BTB.Core.Settings;

/// <summary>
/// Represents the user-specific application settings.
/// </summary>
public class UserSettings
{
    /// <summary>
    /// Gets or sets the culture used by the application user interface.
    /// </summary>
    [Required]
    [RegularExpression(@"^(de-DE|en-US)$")]
    public string UiCulture { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets the application theme.
    /// </summary>
    [Required]
    public AppTheme Theme { get; set; } = AppTheme.Light;
}