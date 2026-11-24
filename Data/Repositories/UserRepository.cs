using BTB.Core.Security;
using BTB.Data.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BTB.Data.Repositories;

/// <summary>
/// Repository implementation for <see cref="User"/> entities.
/// </summary>
public class UserRepository : RepositoryBase<UserRepository>, IUserRepository
{
    #region Private Fields
    private readonly IEncryptionService _encryptionService;
    #endregion

    // Constructor
    public UserRepository(IEncryptionService encryptionService, IDbContextFactory<AppDbContext> dbContextFactory, ILogger<UserRepository> logger) : base(dbContextFactory, logger)
    {
        _encryptionService = encryptionService;
    }

    #region Public Methods
    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    public async Task<Result<List<User>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(
            async (db, ct) =>
            {
                List<User> users = await db.Users
                    .AsNoTracking()
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                        .ToListAsync(ct);
                return Result<List<User>>.Success(users);
            },
            $"Loading users.",
            $"An error occurred while loading the users.",
            cancellationToken);
    }

    /// <summary>
    /// Retrieves a single user by identifier.
    /// </summary>
    public async Task<Result<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(
            async (db, ct) =>
            {
                User? user = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == userId, ct);

                if (user is null)
                {
                    return Result<User>.NotFound($"User {userId} was not found.");
                }
                return Result<User>.Success(user);
            },
            $"Load user {userId}.",
            $"An error occurred while loading the user {userId}.",
            cancellationToken);
    }

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    public async Task<Result<User>> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        return await ExecuteAsync(
            async (db, ct) =>
            {
                db.Users.Add(user);

                await db.SaveChangesAsync(ct);

                return Result<User>.Success(user);
            },
            $"Creating user {user.UserId}",
            $"The user {user.UserId} could not be created.",
            cancellationToken);
    }

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    public Task<Result<User>> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        return ExecuteAsync(
            async (db, ct) =>
            {
                User? existingUser =
                    await db.Users
                        .FirstOrDefaultAsync(
                            x => x.UserId == user.UserId,
                            ct);

                if (existingUser is null)
                {
                    return Result<User>.NotFound($"User {user.UserId} was not found.");
                }

                db.Entry(existingUser)
                  .CurrentValues
                  .SetValues(user);

                await db.SaveChangesAsync(ct);

                return Result<User>.Success(existingUser);
            },
            $"Updating user {user.UserId}.",
            $"The user {user.UserId} could not be updated.",
            cancellationToken);
    }

    /// <summary>
    /// Deletes a user from the database by identifier.
    /// </summary>
    public Task<Result> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(
            async (db, ct) =>
            {
                int affected = await db.Users
                    .Where(x => x.UserId == userId)
                    .ExecuteDeleteAsync(ct);

                if (affected == 0)
                {
                    return Result.NotFound($"User {userId} was not found.");
                }

                return Result.Success();
            },
            $"Deleting user {userId}.",
            $"The user {userId} could not be deleted.",
            cancellationToken);
    }

    /// <summary>
    /// Authenticates a username and password.
    /// </summary>
    public Task<Result<User>> LoginAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(
            async (db, ct) =>
            {
                if (String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(userName))
                {
                    return Result<User>.Unauthorized("Invalid username or password.");
                }

                User? user = await db.Users
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.UserName == userName && x.IsActive, ct);

                if (user is null)
                {
                    return Result<User>.Unauthorized("Invalid username or password.");
                }

                string userPassword = _encryptionService.Decrypt(user.UserPassword);

                if (password != userPassword)
                {
                    return Result<User>.Unauthorized("Invalid username or password.");
                }

                return Result<User>.Success(user);
            },
            $"Login attempt for {userName}.",
            $"An error occurred during login for {userName}.",
            cancellationToken);
    }
    public Task<Result<bool>> IsAuthenticationRequiredAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(
            async (db, ct) =>
            {
                bool required = await db.Users
                    .AsNoTracking()
                    .AnyAsync(
                        user => user.IsActive && !string.IsNullOrWhiteSpace(user.UserPassword),
                        ct);

                return Result<bool>.Success(required);
            },
            "Checking if authentication is required.",
            "Failed to determine whether authentication is required.",
            cancellationToken);
    }
    #endregion
}
