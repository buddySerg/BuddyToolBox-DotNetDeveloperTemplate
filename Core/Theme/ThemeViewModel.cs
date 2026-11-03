using BTB.Core.Settings;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Core.Theme;

/// <summary>
/// Provides the data and behavior required to select and persist the application theme.
/// </summary>
public partial class ThemeViewModel : ObservableObject
{
    #region Private Fields
    private readonly IThemeService _themeService;
    private readonly IUserSettingsService _userSettingsService;
    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the themes available for selection.
    /// </summary>
    public IReadOnlyList<AppTheme> Themes { get; } = Enum.GetValues<AppTheme>();

    /// <summary>
    /// Gets or sets the currently selected application theme.
    /// </summary>
    [ObservableProperty]
    private AppTheme _selectedTheme;
    #endregion

    /// <summary>
    /// Cunstructor
    /// </summary>
    public ThemeViewModel(IThemeService themeService, IUserSettingsService userSettingsService)
    {
        _themeService = themeService;
        _userSettingsService = userSettingsService;

        SelectedTheme = themeService.CurrentTheme;
    }

    #region Events
    /// <summary>
    /// Applies the selected theme and persists the selection to the user settings.
    /// </summary>
    partial void OnSelectedThemeChanged(AppTheme value)
    {
        _themeService.SetTheme(value);
        _ = SaveThemeAsync(value);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Persists the selected theme to the user settings asynchronously.
    /// </summary>
    private async Task SaveThemeAsync(AppTheme theme)
    {
        await _userSettingsService.UpdateAsync(settings =>
        {
            settings.Theme = theme;
        });
    }
    #endregion
}