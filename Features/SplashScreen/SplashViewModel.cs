using System.Reflection;

using BTB.Core.Base;
using BTB.Core.Localization;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Features.SplashScreen;

/// <summary>
/// ViewModel for the splash screen, providing application metadata and status information.
/// </summary>
public sealed partial class SplashViewModel : LocalizedViewModelBase
{
    #region Public Properties
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    private SplashStatus _status;
    public string StatusText => Translate[$"Splash.{Status.Step}"];

    /// <summary>
    /// Gets the application version from assembly attributes.
    /// </summary>
    public string AppVersion { get; }

    /// <summary>
    /// Gets the copyright information for the application.
    /// </summary>
    public string Copyright { get; }
    #endregion Public Properties

    // Constructor
    public SplashViewModel(ILocalizationService localization) : base(localization)
    {
        AppVersion = GetAppVersion();
        Copyright = GetAppCopyright();
        Status = new SplashStatus(StartupStep.StartingServices, 10);
    }

    #region Public Methods
    /// <summary>
    /// Updates the splash status with the current startup step and progress value.
    /// </summary>
    public async Task UpdateStatusAsync(StartupStep step, double progress)
    {
        Status = new SplashStatus(step, progress);
        await Task.Delay(300);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Returns the main application assembly.
    /// </summary>
    private static Assembly GetAppAssembly()
    {
        return Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    }

    /// <summary>
    /// Retrieves the informational version from the assembly attributes.
    /// </summary>
    private static string GetAppVersion()
    {
        string? version = GetAppAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        return version?.Split('+').FirstOrDefault() ?? "Unknown";
    }

    /// <summary>
    /// Retrieves the copyright information from the assembly attributes.
    /// </summary>
    private static string GetAppCopyright()
    {
        return GetAppAssembly()
            .GetCustomAttribute<AssemblyCopyrightAttribute>()
            ?.Copyright
            ?? string.Empty;
    }
    #endregion
}
