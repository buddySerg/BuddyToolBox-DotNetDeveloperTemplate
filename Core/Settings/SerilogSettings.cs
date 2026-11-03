using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Core.Settings;

/// <summary>
/// Defines the root Serilog configuration settings used by the application.
/// </summary>
public sealed partial class SerilogSettings : ObservableObject
{
    /// <summary>
    /// Gets or sets the minimum logging level and category-specific overrides.
    /// </summary>
    [ObservableProperty]
    private MinimumLevelSettings _minimumLevel = new MinimumLevelSettings();

    /// <summary>
    /// Gets or sets the sinks to which log events are written.
    /// </summary>
    [ObservableProperty]
    private List<SerilogSinkSettings> _writeTo = new List<SerilogSinkSettings>();

    /// <summary>
    /// Gets or sets the enrichers applied to log events.
    /// </summary>
    [ObservableProperty]
    private List<string> _enrich = new List<string>();
}

/// <summary>
/// Defines minimum log level settings, including global and source-specific values.
/// </summary>
public sealed partial class MinimumLevelSettings : ObservableObject
{
    /// <summary>
    /// Gets or sets the default minimum logging level.
    /// </summary>
    [ObservableProperty]
    private string _default = "Information";

    /// <summary>
    /// Gets or sets source-specific minimum logging level overrides.
    /// </summary>
    [ObservableProperty]
    private Dictionary<string, string> _override = new Dictionary<string, string>()
    {
        ["Microsoft"] = "Error",
        ["System"] = "Error",
        ["Microsoft.EntityFrameworkCore.Database.Command"] = "Error"
    };
}

/// <summary>
/// Defines the configuration for a Serilog sink entry.
/// </summary>
public sealed class SerilogSinkSettings
{
    /// <summary>
    /// Gets or sets the sink name.
    /// </summary>
    public string Name { get; set; } = "File";

    /// <summary>
    /// Gets or sets the sink arguments for file-based logging.
    /// </summary>
    public FileSinkSettings Args { get; set; } = new FileSinkSettings();
}

/// <summary>
/// Defines the configuration settings for a Serilog file sink.
/// </summary>
public sealed class FileSinkSettings
{
    /// <summary>
    /// Gets or sets the output format used for log entries.
    /// </summary>
    public string OutputTemplate { get; set; } =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level}] => {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Gets or sets the path pattern used for log files.
    /// </summary>
    public string Path { get; set; } = "logs/btb-.log";

    /// <summary>
    /// Gets or sets the interval at which log files are rolled over.
    /// </summary>
    public string RollingInterval { get; set; } = "Day";

    /// <summary>
    /// Gets or sets the maximum number of retained log files.
    /// </summary>
    public int RetainedFileCountLimit { get; set; } = 7;
}
