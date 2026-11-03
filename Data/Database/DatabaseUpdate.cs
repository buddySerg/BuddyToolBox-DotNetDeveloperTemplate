namespace BTB.Data.Database;

/// <summary>
/// Defines the initial database update metadata and SQL script for creating
/// </summary>
public static class DatabaseUpdate
{
    /// <summary>
    /// Gets the unique version number of this database update.
    /// </summary>
    public const int Version = 10000;

    /// <summary>
    /// Gets a short description of the database provider targeted by this update.
    /// </summary>
    public const string Description = "MS SQL";

    /// <summary>
    /// Gets the SQL script
    /// </summary>
    public const string Sql =
@"
CREATE TABLE Users
(
	[UserId] [uniqueidentifier] NOT NULL,
	[Gender] [nvarchar](50) NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[Initials] [nvarchar](10) NULL,
	[Street] [nvarchar](255) NULL,
	[ZipCode] [nvarchar](50) NULL,
	[City] [nvarchar](255) NULL,
	[Country] [nvarchar](50) NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](50) NULL,
	[UserName] [nvarchar](50) NULL,
	[UserPassword] [nvarchar](255) NULL,
	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC)
);
 
ALTER TABLE [Users] ADD CONSTRAINT [DF_Users_UserId] DEFAULT (newid()) FOR [UserId];
";
}