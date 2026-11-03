using BTB.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace BTB.Data;

/// <summary>
/// Represents the Entity Framework database context for the application.
/// </summary>
public class AppDbContext : DbContext
{
    // Constructor
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    #region DbSets
    /// <summary>
    /// Gets the users table mapping.
    /// </summary>
    public DbSet<User> Users => Set<User>();
    #endregion DbSets

    /// <summary>
    /// Configures entity mappings and model constraints.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasKey(x => x.UserId);
    }
}