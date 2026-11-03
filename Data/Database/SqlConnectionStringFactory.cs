using BTB.Core.Security;
using BTB.Core.Settings;

using Microsoft.Data.SqlClient;

namespace BTB.Data.Database;

/// <summary>
/// Creates SQL Server connection strings from SQL configuration settings.
/// </summary>
public sealed class SqlConnectionStringFactory : IConnectionStringFactory
{
    private readonly IEncryptionService _encryptionService;


    // Constructor
    public SqlConnectionStringFactory(IEncryptionService encryptionService)
    {
        ArgumentNullException.ThrowIfNull(encryptionService);

        _encryptionService = encryptionService;
    }

    /// <summary>
    /// Builds a SQL Server connection string using integrated or credential-based authentication.
    /// </summary>
    public string Create(SqlSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        SqlConnectionStringBuilder builder = new()
        {
            DataSource = settings.Server,
            InitialCatalog = settings.Database,
            IntegratedSecurity = settings.UseIntegratedSecurity,
            Encrypt = settings.Encrypt,
            TrustServerCertificate = settings.TrustServerCertificate
        };

        if (!settings.UseIntegratedSecurity)
        {
            builder.UserID = settings.User;
            builder.Password = _encryptionService.Decrypt(settings.EncryptedPassword) ?? string.Empty;
        }

        return builder.ConnectionString;
    }
}
