using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Core.Settings;

/// <summary>
/// Defines the Microsoft SQL Server connection settings used by the application.
/// </summary>
public sealed partial class SqlSettings : ObservableObject
{
    /// <summary>
    /// Gets or sets the name of the SQL Server instance.
    /// </summary>
    [ObservableProperty]
    private string _server = @"localhost\SQLEXPRESS";

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    [ObservableProperty]
    private string _database = "BTB_DeveloperTemplate";

    /// <summary>
    /// Gets or sets a value indicating whether integrated Windows authentication is used.
    /// </summary>
    [ObservableProperty]
    private bool _useIntegratedSecurity = true;

    /// <summary>
    /// Gets or sets the SQL Server user name.
    /// </summary>
    [ObservableProperty]
    private string? _user;

    /// <summary>
    /// Gets or sets the encrypted SQL Server password.
    /// </summary>
    [ObservableProperty]
    private string? _encryptedPassword;

    /// <summary>
    /// Gets or sets a value indicating whether the SQL Server certificate is trusted.
    /// </summary>
    [ObservableProperty]
    private bool _trustServerCertificate = true;

    /// <summary>
    /// Gets or sets a value indicating whether the connection is encrypted.
    /// </summary>
    [ObservableProperty]
    private bool _encrypt = true;
}