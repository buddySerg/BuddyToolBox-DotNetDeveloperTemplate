using System.ComponentModel;

using BTB.Core.Base;
using BTB.Core.Dialogs;
using BTB.Core.Helpers;
using BTB.Core.Localization;
using BTB.Core.Navigation;
using BTB.Core.Repositories;
using BTB.Core.Security;
using BTB.Core.Settings;
using BTB.Data.Database;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BTB.Features.Settings
{
    /// <summary>
    /// Coordinates loading, editing, validating, and saving application settings.
    /// </summary>
    public sealed partial class SettingsViewModel : LocalizedViewModelBase, INavigationAware
    {
        #region Private Fields
        private readonly IJsonRepository<ApplicationSettings> _repository;
        private readonly IDialogService _dialogService;
        private readonly ILogger<SettingsViewModel> _logger;
        private ApplicationSettings _originalSettings = default!;
        #endregion

        #region Public Properties
        /// <summary>
        /// Provides access to general application settings.
        /// </summary>
        public GeneralSettingsViewModel General { get; }

        /// <summary>
        /// Provides access to database-related settings.
        /// </summary>
        public DatabaseSettingsViewModel Database { get; }

        /// <summary>
        /// Provides access to logging-related settings.
        /// </summary>
        public LoggingSettingsViewModel Logging { get; }

        [ObservableProperty]
        private ApplicationSettings _settings = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        private bool _hasChanges;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        private bool _isBusy;
        #endregion

        #region Constructor
        public SettingsViewModel(
            IOptions<ApplicationSettings> settings,
            ILocalizationService localization,
            IJsonRepository<ApplicationSettings> repository,
            IEncryptionService encryptionService, IConnectionStringFactory connectionStringFactory,
            IDialogService dialogService,
            ILogger<SettingsViewModel> logger) : base(localization)
        {
            _repository = repository;
            _dialogService = dialogService;
            _logger = logger;

            Settings = settings.Value;
            General = new GeneralSettingsViewModel(Settings.AppSettings);
            Logging = new LoggingSettingsViewModel(Settings.Serilog);
            Database = new DatabaseSettingsViewModel(Settings.Sql, localization, encryptionService, connectionStringFactory, _dialogService);

            SubscribeSettings(Settings);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads the latest persisted settings when navigation enters this view.
        /// </summary>
        public async Task OnNavigatedToAsync(RouteContext context, CancellationToken cancellationToken = default)
        {
            await LoadSettingsAsync();
        }

        /// <summary>
        /// Releases change notifications when navigation leaves this view.
        /// </summary>
        public Task OnNavigatedFromAsync(RouteContext context, CancellationToken cancellationToken = default)
        {
            UnsubscribeSettings(Settings);
            return Task.CompletedTask;
        }
        #endregion

        #region Events
        /// <summary>
        /// Re-evaluates the dirty state whenever any tracked settings property changes.
        /// </summary>
        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            HasChanges = !ObjectComparer.AreEqual(_originalSettings, Settings);
        }
        #endregion

        #region Event Subscription
        /// <summary>
        /// Subscribes to property change notifications for all relevant settings nodes.
        /// </summary>
        private void SubscribeSettings(ApplicationSettings settings)
        {
            if (settings is null)
            {
                return;
            }

            UnsubscribeSettings(settings);
            settings.PropertyChanged += OnSettingsPropertyChanged;
            settings.AppSettings?.PropertyChanged += OnSettingsPropertyChanged;
            settings.Sql?.PropertyChanged += OnSettingsPropertyChanged;
            settings.Serilog?.PropertyChanged += OnSettingsPropertyChanged;
            settings.Serilog?.MinimumLevel?.PropertyChanged += OnSettingsPropertyChanged;
        }

        /// <summary>
        /// Unsubscribes from property change notifications for all relevant settings nodes.
        /// </summary>
        private void UnsubscribeSettings(ApplicationSettings settings)
        {
            if (settings is null)
            {
                return;
            }

            settings.PropertyChanged -= OnSettingsPropertyChanged;
            settings.AppSettings?.PropertyChanged -= OnSettingsPropertyChanged;
            settings.Sql?.PropertyChanged -= OnSettingsPropertyChanged;
            settings.Serilog?.PropertyChanged -= OnSettingsPropertyChanged;
            settings.Serilog?.MinimumLevel?.PropertyChanged -= OnSettingsPropertyChanged;
        }
        #endregion

        #region Commands
        /// <summary>
        /// Persists current settings, including preprocessing of database configuration.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSave), AllowConcurrentExecutions = false)]
        private async Task SaveAsync()
        {
            try
            {
                IsBusy = true;

                // Prepare database settings, including password encryption.
                _logger.LogDebug("Preparing database settings.");
                Database.PrepareSettings();

                await _repository.SaveAsync(Settings);

                ResetChangeTracking();

                _logger.LogInformation("Application settings saved successfully.");

                await _dialogService.ShowInfo(Translate["Features.Settings.ApplicationSettingsSavedSuccessfully"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save application settings.");

                await _dialogService.ShowError(Translate["Features.Settings.ApplicationSettingsSaveFailed"]);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Reloads settings from the repository, optionally confirming when unsaved changes exist.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanRefresh), AllowConcurrentExecutions = false)]
        private async Task RefreshAsync()
        {
            if (HasChanges)
            {
                bool confirmed = await _dialogService.Confirm(Translate["Features.Settings.RefreshConfirmation"]);

                if (!confirmed)
                {
                    return;
                }
            }

            try
            {
                IsBusy = true;
                await LoadSettingsAsync();
                await _dialogService.ShowInfo(Translate["Features.Settings.ApplicationSettingsReloaded"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh application settings.");
                await _dialogService.ShowError(Translate["Features.Settings.ApplicationSettingsRefreshFailed"]);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Determines whether saving is currently allowed.
        /// </summary>
        private bool CanSave()
        {
            return HasChanges && !IsBusy;
        }

        /// <summary>
        /// Determines whether refreshing is currently allowed.
        /// </summary>
        private bool CanRefresh()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Resets dirty-state tracking based on the current settings snapshot.
        /// </summary>
        private void ResetChangeTracking()
        {
            _originalSettings = _repository.Clone(Settings);
            HasChanges = false;
        }

        /// <summary>
        /// Loads settings from the repository and propagates them to child view models.
        /// </summary>
        private async Task LoadSettingsAsync()
        {
            try
            {
                ApplicationSettings loadedSettings = await _repository.LoadAsync();

                if (loadedSettings is null)
                {
                    _logger.LogWarning("The settings repository returned no settings.");
                    await _dialogService.ShowError(Translate["Features.Settings.ApplicationSettingsCouldNotBeLoaded"]);
                    return;
                }

                UnsubscribeSettings(Settings);
                Settings = loadedSettings;
                SubscribeSettings(Settings);

                ResetChangeTracking();

                General.Settings = Settings.AppSettings;
                Database.Settings = Settings.Sql;
                Logging.Settings = Settings.Serilog;

                _logger.LogDebug("Application settings loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load application settings.");
                await _dialogService.ShowError(Translate["Features.Settings.ApplicationSettingsCouldNotBeLoaded"]);
            }
        }
        #endregion
    }
}