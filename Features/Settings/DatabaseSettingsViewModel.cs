using BTB.Core.Base;
using BTB.Core.Dialogs;
using BTB.Core.Localization;
using BTB.Core.Security;
using BTB.Core.Settings;
using BTB.Data.Database;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Data.SqlClient;

namespace BTB.Features.Settings;

/// <summary>
/// Provides database connection settings and connection testing functionality.
/// </summary>
public partial class DatabaseSettingsViewModel : LocalizedViewModelBase
{

    #region Private Fields
    private readonly IDialogService _dialogService;
    private readonly IEncryptionService _encryptionService;
    private readonly IConnectionStringFactory _connectionStringFactory;
    #endregion

    #region Observable Properties
    [ObservableProperty]
    private SqlSettings _settings = new SqlSettings();

    [ObservableProperty]
    private string _password = string.Empty;
    #endregion

    // Constructor
    public DatabaseSettingsViewModel(SqlSettings settings, ILocalizationService localization, IEncryptionService encryptionService, IConnectionStringFactory connectionStringFactory, IDialogService dialogService) : base(localization)
    {
        _encryptionService = encryptionService;
        _connectionStringFactory = connectionStringFactory;
        _dialogService = dialogService;

        Settings = settings;
    }

    #region Public Methods
    /// <summary>
    /// Prepares the database settings for persistence by applying the current authentication
    /// mode and encrypting the entered password when required.
    /// </summary>
    public void PrepareSettings()
    {
        if (Settings.UseIntegratedSecurity)
        {
            Settings.User = null;
            Settings.EncryptedPassword = null;
        }
        else if (!string.IsNullOrWhiteSpace(Password))
        {
            Settings.EncryptedPassword = _encryptionService.Encrypt(Password);
        }

        Password = string.Empty;
    }
    #endregion

    #region Commands
    [RelayCommand]
    /// <summary>
    /// Tests the database connection
    /// </summary>
    private async Task TestConnectionAsync()
    {
        try
        {
            SqlSettings testSettings = new()
            {
                Server = Settings.Server,
                Database = Settings.Database,
                User = Settings.User,
                UseIntegratedSecurity = Settings.UseIntegratedSecurity,
                Encrypt = Settings.Encrypt,
                TrustServerCertificate = Settings.TrustServerCertificate
            };

            if (!testSettings.UseIntegratedSecurity && !string.IsNullOrWhiteSpace(Password))
            {
                testSettings.EncryptedPassword = _encryptionService.Encrypt(Password);
            }
            else
            {
                testSettings.EncryptedPassword = Settings.EncryptedPassword;
            }

            using SqlConnection connection = new SqlConnection(_connectionStringFactory.Create(testSettings));

            await connection.OpenAsync();

            using SqlCommand command = new SqlCommand("SELECT @@VERSION", connection);

            string sqlVersion = Convert.ToString(await command.ExecuteScalarAsync()) ?? "Unknown";

            await _dialogService.ShowInfo(Translate.GetString("Features.Settings.ConnectionSuccessful", Settings.Server, Settings.Database, sqlVersion));
        }
        catch (SqlException ex)
        {
            await _dialogService.ShowError(Translate.GetString("Features.Settings.ConnectionFailed", ex.Message, ex.Number));
        }
        catch (Exception ex)
        {
            await _dialogService.ShowError(Translate.GetString("Features.Settings.ConnectionFailed", ex.Message, string.Empty));
        }
    }
    #endregion
}