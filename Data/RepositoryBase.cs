using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BTB.Data;

/// <summary>
/// Provides shared repository infrastructure for database context creation,execution wrappers, and consistent exception handling.
/// </summary>
public abstract class RepositoryBase<TRepository>
{
    #region Fields
    /// <summary>
    /// Creates new database context instances for repository operations.
    /// </summary>
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    /// <summary>
    /// Writes repository diagnostics and error information.
    /// </summary>
    private readonly ILogger<TRepository> _logger;
    #endregion

    // Constructors
    protected RepositoryBase(IDbContextFactory<AppDbContext> dbContextFactory, ILogger<TRepository> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    #region methods
    /// <summary>
    /// Creates a new <see cref="AppDbContext"/> instance for the current operation.
    /// </summary>
    protected async Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContextFactory.CreateDbContextAsync(cancellationToken);
    }

    /// <summary>
    /// Executes a data operation with standardized exception handling and logging, returning a typed result.
    /// </summary>
    protected async Task<Result<T>> ExecuteAsync<T>(
        Func<AppDbContext, CancellationToken, Task<Result<T>>> action,
        string actionDescription,
        string exceptionMessage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using AppDbContext db = await CreateDbContextAsync(cancellationToken);

            return await action(db, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation("Operation {ActionDescription} canceled. Message: {Message}", actionDescription, ex.Message);
            return Result<T>.Canceled($"{exceptionMessage} ({ex.Message})");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error during {ActionDescription}. Message: {Message}", actionDescription, ex.Message);
            return Result<T>.Failure($"{exceptionMessage} ({ex.Message})");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {ActionDescription}. Message: {Message}", actionDescription, ex.Message);
            return Result<T>.Failure($"{exceptionMessage} ({ex.Message})");
        }
    }

    /// <summary>
    /// Executes a data operation with standardized exception handling and logging, returning a non-generic result.
    /// </summary>
    protected async Task<Result> ExecuteAsync(
        Func<AppDbContext, CancellationToken, Task<Result>> action,
        string actionDescription,
        string exceptionMessage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using AppDbContext db = await CreateDbContextAsync(cancellationToken);

            return await action(db, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug("Operation {ActionDescription} canceled. Message: {Message}", actionDescription, ex.Message);
            return Result.Canceled();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error during {ActionDescription}. Message: {Message}", actionDescription, ex.Message);
            return Result.Failure($"{exceptionMessage}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {ActionDescription}. Message: {Message}", actionDescription, ex.Message);
            return Result.Failure($"{exceptionMessage}");
        }
    }
    #endregion
}
