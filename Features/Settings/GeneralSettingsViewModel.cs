using BTB.Core.Settings;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Features.Settings
{
    /// <summary>
    /// Provides state and behavior for editing general application settings.
    /// </summary>
    public partial class GeneralSettingsViewModel : ObservableObject
    {
        /// <summary>
        /// Stores the application settings.
        /// </summary>
        [ObservableProperty]
        private AppSettings _settings;

        // Constructor
        public GeneralSettingsViewModel(AppSettings settings)
        {
            Settings = settings;
        }
    }
}
