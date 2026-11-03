using System.Reflection;

using BTB.Core.Base;
using BTB.Core.Localization;
using BTB.Core.Theme;
using BTB.Features.Navigation;

using Microsoft.Extensions.Logging;

namespace BTB.Core
{
    public partial class MainViewModel : LocalizedViewModelBase
    {

        #region Private fields
        private readonly ILogger<MainViewModel> _logger;
        #endregion

        #region Public properties
        public ThemeViewModel ThemeViewModel { get; }
        public LocalizationViewModel LocalizationViewModel { get; }
        public NavigationViewModel NavigationViewModel { get; }
        public string? AppVersion { get; }
        #endregion

        /// <summary>
        /// Constructor for MainViewModel.
        /// </summary>
        public MainViewModel(
            ILocalizationService localization,
            ThemeViewModel theme,
            NavigationViewModel navigationViewModel,
            ILogger<MainViewModel> logger,
            LocalizationViewModel localizationViewModel)
        : base(localization)
        {
            _logger = logger;
            ThemeViewModel = theme;
            NavigationViewModel = navigationViewModel;
            LocalizationViewModel = localizationViewModel;


            AppVersion = GetAppVersion();
            _logger.LogDebug("MainViewModel created with version {Version}", AppVersion);
        }

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