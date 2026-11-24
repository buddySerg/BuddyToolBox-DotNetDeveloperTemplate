using BTB.Data.Entities;

namespace BTB.Data.Repositories;

/// <summary>
/// Provides persistence operations for <see cref="User"/> entities.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves all users ordered by last name and first name.
    /// </summary>
    Task<Result<List<User>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by its identifier.
    /// </summary>
    Task<Result<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user in the data store.
    /// </summary>
    Task<Result<User>> AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user in the data store.
    /// </summary>
    Task<Result<User>> UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user by its identifier.
    /// </summary>
    Task<Result> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user by username and password.
    /// </summary>
    Task<Result<User>> LoginAsync(string userName, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines asynchronously whether authentication is required.
    /// </summary>
    Task<Result<bool>> IsAuthenticationRequiredAsync(CancellationToken cancellationToken = default);
}