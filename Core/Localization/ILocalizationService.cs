using System.ComponentModel;
using System.Globalization;

namespace BTB.Core.Localization;

/// <summary>
/// Provides localized strings and manages the application's current UI culture.
/// </summary>
public interface ILocalizationService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the currently active localization culture.
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    /// Occurs when the active localization culture changes.
    /// </summary>
    event EventHandler? CultureChanged;

    /// <summary>
    /// Gets the localized string associated with the specified resource key.
    /// </summary>
    /// <returns>The localized string, or a fallback value if the key is missing.</returns>
    string this[string key] { get; }

    /// <summary>
    /// Retrieves a localized string by its resource key.
    /// </summary>
    /// <returns>The localized string, or a fallback value if the key is missing.</returns>
    string GetString(string key);

    string GetString(string key, params object[] args);

    /// <summary>
    /// Changes the active localization culture.
    /// </summary>
    void SetCulture(CultureInfo cultureInfo);
}