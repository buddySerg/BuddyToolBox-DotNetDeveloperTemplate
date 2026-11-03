using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace BTB.Data.Database;

/// <summary>
/// Coordinates one-time database update execution and records applied update versions.
/// </summary>
public sealed class DatabaseUpdateService
{
    #region Private Fields
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly ILogger<DatabaseUpdateService> _logger;
    #endregion

    #region Constructor
    public DatabaseUpdateService(IDbContextFactory<AppDbContext> factory, ILogger<DatabaseUpdateService> logger)
    {
        _factory = factory;
        _logger = logger;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Ensures the version table exists and applies the current update if it has not been applied yet.
    /// </summary>
    public async Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        await using AppDbContext db = await _factory.CreateDbContextAsync(cancellationToken);

        await CreateVersionTableAsync(db, cancellationToken);

        bool applied = await db.Database.SqlQuery<int>(
            $"SELECT Version AS Value FROM dbo.DatabaseVersions WHERE Version = {DatabaseUpdate.Version}")
            .AnyAsync(cancellationToken);

        if (applied)
        {
            return;
        }

        await ApplyUpdateAsync(db, cancellationToken);

    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Executes the configured database update and persists the applied version in a single transaction.
    /// </summary>
    private async Task ApplyUpdateAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _logger.LogInformation("Applying database update {Version}: {Description}", DatabaseUpdate.Version, DatabaseUpdate.Description);

            await db.Database.ExecuteSqlRawAsync(DatabaseUpdate.Sql, cancellationToken);

            await db.Database.ExecuteSqlAsync(
                $"INSERT INTO dbo.DatabaseVersions (Version, Description) VALUES ({DatabaseUpdate.Version}, {DatabaseUpdate.Description}) ",
                cancellationToken
            );

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully applied database update {Version}: {Description}", DatabaseUpdate.Version, DatabaseUpdate.Description);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError("Database update {Version} failed: {Description}", DatabaseUpdate.Version, DatabaseUpdate.Description);
            throw;
        }
    }

    /// <summary>
    /// Creates the database version tracking table when it does not already exist.
    /// </summary>
    private static async Task CreateVersionTableAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        const string sql =
        @"
            IF OBJECT_ID('dbo.DatabaseVersions', 'U') IS NULL
            BEGIN
            CREATE TABLE dbo.DatabaseVersions
            (
                Version int NOT NULL,
                Description nvarchar(255) NOT NULL,
                UpdatedAt datetime NOT NULL CONSTRAINT DF_UpdatedAt DEFAULT SYSUTCDATETIME(),

                CONSTRAINT PK_DatabaseVersions PRIMARY KEY (Version)
            );
            END;
            ";

        await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }
    #endregion
}