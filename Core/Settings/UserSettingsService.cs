using BTB.Core.Repositories;

namespace BTB.Core.Settings;

/// <summary>
/// Provides thread-safe loading and persistence of user settings.
/// </summary>
public sealed class UserSettingsService : IUserSettingsService
{

    #region Private Fields
    /// <summary>
    /// Repository used to load and save user settings as JSON.
    /// </summary>
    private readonly IJsonRepository<UserSettings> _repository;

    /// <summary>
    /// Synchronizes concurrent access to settings load and update operations.
    /// </summary>
    private readonly SemaphoreSlim _lock = new(1, 1);
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the currently loaded user settings.
    /// </summary>
    public UserSettings CurrentSettings { get; private set; } = new();
    #endregion


    // Constructor
    public UserSettingsService(IJsonRepository<UserSettings> repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
    }

    #region public Methods
    /// <summary>
    /// Loads the user settings from the configured repository.
    /// </summary>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            CurrentSettings = await _repository.LoadAsync(cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Applies an update to the current user settings and persists the changes.
    /// </summary>
    public async Task UpdateAsync(Action<UserSettings> update, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(update);

        await _lock.WaitAsync(cancellationToken);

        try
        {
            update(CurrentSettings);

            await _repository.SaveAsync(CurrentSettings, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }
    #endregion
}
