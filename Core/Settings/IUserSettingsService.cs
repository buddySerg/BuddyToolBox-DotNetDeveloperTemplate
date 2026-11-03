namespace BTB.Core.Settings;

/// <summary>
/// Defines methods for loading and updating user settings.
/// </summary>
public interface IUserSettingsService
{
    #region Public Properties
    /// <summary>
    /// Gets the currently loaded user settings.
    /// </summary>
    UserSettings CurrentSettings { get; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Loads settings from the repository and updates the current settings.
    /// </summary>
    Task LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies updates to the current settings and persists them in the repository.
    /// </summary>
    Task UpdateAsync(Action<UserSettings> update, CancellationToken cancellationToken = default);
    #endregion
}
