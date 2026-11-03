using System.Globalization;

using BTB.Core.Settings;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Core.Localization;

/// <summary>
/// Provides the state and behavior for selecting and persisting the application's user interface culture.
/// </summary>
public partial class LocalizationViewModel : ObservableObject
{
    #region Private Fields
    private readonly ILocalizationService _localizationService;
    private readonly IUserSettingsService _userSettingsService;
    #endregion

    #region Public Properties
    /// <summary>
    /// Represents a culture that can be selected for the user interface.
    /// </summary>
    public sealed record CultureItem(string DisplayName, string Culture);

    /// <summary>
    /// Gets the cultures available for selection.
    /// </summary>
    public IReadOnlyList<CultureItem> Cultures { get; } =
    [
        new(new CultureInfo("de-DE").NativeName.Split('(')[0].Trim(),"de-DE"),
        new(new CultureInfo("en-US").NativeName.Split('(')[0].Trim(), "en-US")
    ];

    /// <summary>
    /// Gets or sets the currently selected user interface culture.
    /// </summary>
    [ObservableProperty]
    private CultureItem? _selectedCulture;
    #endregion

    #region Constructors
    public LocalizationViewModel(ILocalizationService localizationService, IUserSettingsService userSettingsService)
    {
        _localizationService = localizationService;
        _userSettingsService = userSettingsService;

        SelectedCulture = Cultures.FirstOrDefault(x => x.Culture == localizationService.CurrentCulture.Name) ?? Cultures.First();
    }
    #endregion

    #region Events
    /// <summary>
    /// Applies and persists the selected culture when it changes.
    /// </summary>
    partial void OnSelectedCultureChanged(CultureItem? value)
    {
        if (value is null || value.Culture == _userSettingsService.CurrentSettings.UiCulture)
        {
            return;
        }

        _localizationService.SetCulture(CultureInfo.GetCultureInfo(value.Culture));

        _ = SaveCultureAsync(value.Culture);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Persists the specified culture in the user settings.
    /// </summary>
    private async Task SaveCultureAsync(string culture)
    {
        await _userSettingsService.UpdateAsync(settings =>
        {
            settings.UiCulture = culture;
        });
    }
    #endregion
}