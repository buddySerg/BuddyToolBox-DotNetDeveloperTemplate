using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace BTB.Core.Localization;

/// <summary>
/// Provides access to localized resources and manages the culture used for localization.
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    #region Private Fields
    /// <summary>
    /// The resource manager used to retrieve localized strings from the resource files.
    /// </summary>
    private static readonly ResourceManager _resourceManager = Resources.Languages.Strings.ResourceManager;
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the current culture used for localization.
    /// </summary>
    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentUICulture;

    /// <summary>
    /// Occurs when the culture is changed in the localization service.
    /// </summary>
    public event EventHandler? CultureChanged;

    /// <summary>
    /// Occurs when a property value changes. This event is raised when the current culture changes, allowing UI elements to update their displayed text.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    /// <summary>
    /// Gets the localized string associated with the specified resource key.
    /// </summary>
    public string this[string key] => GetString(key);

    /// <summary>
    /// Gets the localized string associated with the specified resource key.
    /// If the resource key is not found, a marker indicating the missing translation is returned.
    /// </summary>
    public string GetString(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return _resourceManager.GetString(key, CurrentCulture) ?? $"<TR: {key}>";
    }

    /// <summary>
    /// Gets the localized string associated with the specified resource key and formats it with the supplied arguments.
    /// If the resource key is not found, a marker indicating the missing translation is used as the format string.
    /// </summary>
    public string GetString(string key, params object[] args)
    {
        string template = GetString(key);

        return args.Length > 0
            ? string.Format(CurrentCulture, template, args)
            : template;
    }

    /// <summary>
    /// Sets the culture used for localization.
    /// If the specified culture differs from the current culture, thread cultures are updated and notifications are raised for dependent bindings and subscribers.
    /// </summary>
    public void SetCulture(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);

        if (cultureInfo.Equals(CurrentCulture))
        {
            return;
        }

        CurrentCulture = cultureInfo;

        // Current UI thread
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        // Threads created afterwards
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        // Notify bindings that depend on the current culture.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));

        // Refresh all WPF indexer bindings: Translate[Some.Resource.Key]
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));

        // Important for bindings using the indexer: [Key]
        CultureChanged?.Invoke(this, EventArgs.Empty);
    }
}