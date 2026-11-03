using BTB.Core.Settings;

namespace BTB.Data.Database;

/// <summary>
/// Defines a factory for creating database connection strings.
/// </summary>
public interface IConnectionStringFactory
{
    /// <summary>
    /// Creates a connection string from SQL configuration settings.
    /// </summary>
    string Create(SqlSettings settings);
}