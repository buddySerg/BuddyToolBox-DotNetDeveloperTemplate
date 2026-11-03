using BTB.Core.Localization;

namespace BTB.Core.Base;

/// <summary>
/// Provides a base implementation for view models that require localized content.
/// </summary>
public abstract class LocalizedViewModelBase : ViewModelBase
{
    /// <summary>
    /// The localization service used to retrieve localized strings and handle culture changes.
    /// </summary>
    private readonly ILocalizationService _localization;

    /// <summary>
    /// Gets the localization service used by the view model.
    /// </summary>
    public ILocalizationService Translate => _localization;

    /// <summary>
    /// Constructor
    /// </summary>
    protected LocalizedViewModelBase(ILocalizationService localization)
    {
        ArgumentNullException.ThrowIfNull(localization);

        _localization = localization;
        _localization.CultureChanged += HandleCultureChanged;
    }

    /// <summary>
    /// Retrieves a localized string for the specified key.
    /// </summary>
    protected string GetTranslation(string key) => _localization.GetString(key);

    /// <summary>
    /// Handles culture changes raised by the localization service.
    /// </summary>
    private void HandleCultureChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(Translate));
        OnCultureChanged();
    }

    /// <summary>
    /// Called when the application culture changes.
    /// Override this method in derived view models to refresh properties containing cached localized strings.
    /// </summary>
    protected virtual void OnCultureChanged()
    {
    }

    /// <summary>
    /// Releases the resources used by the view model.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _localization.CultureChanged -= HandleCultureChanged;
        }

        base.Dispose(disposing);
    }
}