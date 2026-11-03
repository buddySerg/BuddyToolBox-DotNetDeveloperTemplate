namespace BTB.Core.Theme;

public interface IThemeService
{
    /// <summary>
    /// Gets the current theme of the application.
    /// </summary>
    AppTheme CurrentTheme { get; }

    /// <summary>
    /// Sets the theme of the application.
    /// </summary>
    void SetTheme(AppTheme theme);
}