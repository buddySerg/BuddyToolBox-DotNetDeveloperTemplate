using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Core.Settings;

/// <summary>
/// Represents the root configuration object that groups all application-related settings sections.
/// </summary>
public partial class ApplicationSettings : ObservableObject
{
    /// <summary>
    /// Gets or sets the general application settings.
    /// </summary>
    [ObservableProperty]
    private AppSettings _appSettings = new AppSettings();

    /// <summary>
    /// Gets or sets the SQL database settings.
    /// </summary>
    [ObservableProperty]
    private SqlSettings _sql = new SqlSettings();

    /// <summary>
    /// Gets or sets the Serilog logging settings.
    /// </summary>
    [ObservableProperty]
    private SerilogSettings _serilog = new SerilogSettings();
}
