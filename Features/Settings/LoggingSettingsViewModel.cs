using BTB.Core.Settings;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Features.Settings;

/// <summary>
/// Provides observable data for configuring application logging.
/// </summary>
public partial class LoggingSettingsViewModel : ObservableObject
{
    /// <summary>
    /// Gets the logging levels available for selection.
    /// </summary>
    public static IReadOnlyList<string> Levels =>
    [
        "Verbose",
        "Debug",
        "Information",
        "Warning",
        "Error",
        "Fatal"
    ];

    /// <summary>
    /// Gets or sets the Serilog configuration.
    /// </summary>
    [ObservableProperty]
    private SerilogSettings _settings;

    // Constructor
    public LoggingSettingsViewModel(SerilogSettings settings)
    {
        Settings = settings;
    }
}
