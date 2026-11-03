using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Core.Settings;

/// <summary>
/// Represents application-level settings that can be observed for changes.
/// </summary>
public sealed partial class AppSettings : ObservableObject
{
    /// <summary>
    /// Gets or sets the maximum number of items displayed by the application.
    /// </summary>
    [ObservableProperty]
    private int _maxItems = 1000;
}
