using System.Reflection;

using BTB.Core.Base;
using BTB.Core.Dialogs;
using BTB.Core.Localization;
using BTB.Core.Theme;
using BTB.Features.Navigation;
using BTB.Features.Users;

using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BTB.Core
{
    public partial class MainViewModel : LocalizedViewModelBase
    {

        #region Private fields
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MainViewModel> _logger;
        private readonly IDialogService _dialogService;
        private readonly ApplicationState _applicationState;
        #endregion

        #region Public properties
        public ThemeViewModel ThemeViewModel { get; }
        public LocalizationViewModel LocalizationViewModel { get; }
        public NavigationViewModel NavigationViewModel { get; }
        public string? AppVersion { get; }
        public bool IsLoggedIn => _applicationState.IsLoggedIn;
        public string UserNameDisplay => _applicationState.CurrentUser?.FullName ?? "Guest";
        public bool ShowUserName => _applicationState.IsAuthenticationRequired && _applicationState.IsLoggedIn;
        public bool ShowLoginButton => _applicationState.IsAuthenticationRequired && !_applicationState.IsLoggedIn;
        #endregion

        /// <summary>
        /// Constructor for MainViewModel.
        /// </summary>
        public MainViewModel(
            IServiceProvider serviceProvider,
            ILocalizationService localization,
            ThemeViewModel theme,
            NavigationViewModel navigationViewModel,
            ILogger<MainViewModel> logger,
            LocalizationViewModel localizationViewModel, IDialogService dialogService,
            ApplicationState applicationState)
        : base(localization)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _dialogService = dialogService;
            _applicationState = applicationState;
            ThemeViewModel = theme;
            NavigationViewModel = navigationViewModel;
            LocalizationViewModel = localizationViewModel;

            AppVersion = GetAppVersion();
            _applicationState.CurrentUserChanged += OnCurrentUserChanged;

            _logger.LogDebug("MainViewModel created with version {Version}", AppVersion);
        }

        /// <summary>
        /// Login
        /// </summary>
        [RelayCommand]
        private async Task LoginAsync()
        {
            UserLoginViewModel? userLoginViewModel = _serviceProvider.GetRequiredService<UserLoginViewModel>();

            await _dialogService.Show(
                userLoginViewModel,
                Translate["Features.Users.Login"],
                new DialogOptions
                {
                    PrimaryButtonText = Translate["Features.Users.Login"],
                    PrimaryButtonValidation = userLoginViewModel.ValidateLoginAsync
                });
        }

        [RelayCommand]
        private void Logout()
        {
            _applicationState.Logout();
            _logger.LogInformation("User logged out.");
        }

        #region Event Handling
        private void OnCurrentUserChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(UserNameDisplay));
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(ShowUserName));
            OnPropertyChanged(nameof(ShowLoginButton));
        }
        #endregion

        #region Private methods
        private static string? GetAppVersion()
        {
            return Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion?
                .Split('+')[0];
        }
        #endregion
    }
}