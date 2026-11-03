using System.Collections.ObjectModel;
using System.Windows;

using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace BTB.Core.Theme;

/// <summary>
/// Provides theme management for the application and its WPF UI controls.
/// </summary>
public sealed class ThemeService : IThemeService
{
    /// <summary>
    /// Defines the relative path to the application's custom theme resources.
    /// </summary>
    private const string ThemePath = "/Resources/Themes/";

    #region Public Properties
    /// <summary>
    /// Gets the theme currently applied to the application.
    /// </summary>
    public AppTheme CurrentTheme { get; private set; } = AppTheme.Light;
    #endregion

    /// <summary>
    /// Applies the specified theme to the application, including WPF UI controls and the application's custom resource dictionaries.
    /// </summary>
    public void SetTheme(AppTheme theme)
    {
        CurrentTheme = theme;

        ApplicationTheme wpfTheme = theme switch
        {
            AppTheme.Dark => ApplicationTheme.Dark,
            AppTheme.Light => ApplicationTheme.Light,
            _ => ApplicationTheme.Light
        };

        // Theme for WPF UI Controls
        ApplicationThemeManager.Apply(wpfTheme, WindowBackdropType.Mica);
        ApplyCustomTheme(theme);
    }

    /// <summary>
    /// Applies the specified custom theme by updating the application's merged resource dictionaries.
    /// </summary>
    private static void ApplyCustomTheme(AppTheme theme)
    {
        Collection<ResourceDictionary> dictionaries = Application.Current.Resources.MergedDictionaries;

        ResourceDictionary? oldTheme = dictionaries
            .FirstOrDefault(dictionary => dictionary.Source?.OriginalString.Contains("/Resources/Themes/Theme.", StringComparison.OrdinalIgnoreCase) == true
            && !dictionary.Source.OriginalString.EndsWith("Theme.Shared.xaml", StringComparison.OrdinalIgnoreCase));

        if (oldTheme is not null)
        {
            dictionaries.Remove(oldTheme);
        }

        dictionaries.Add(new ResourceDictionary
        {
            Source = new Uri($"{ThemePath}Theme.{theme}.xaml", UriKind.Relative)
        });
    }
}
