namespace BTB.Data.Database;

/// <summary>
/// Defines the initial database update metadata and the SQL script required to initialize or update the database.
/// </summary>
public static class DatabaseUpdate
{
    /// <summary>
    /// The unique version number of the latest database update.
    /// </summary>
    public const int Version = 10001;

    /// <summary>
    /// Description of the database provider.
    /// </summary>
    public const string Description = "Microsoft SQL Server";

    /// <summary>
    /// Update SQL script
    /// </summary>
    public const string Sql =
@"
-- Version 10000
IF OBJECT_ID('Users', 'U') IS NULL
	BEGIN
		CREATE TABLE Users
		(
			[UserId] [uniqueidentifier] NOT NULL CONSTRAINT DF_Users_UserId DEFAULT NEWID(),
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
	END

-- Version 10001
ALTER TABLE Users
ALTER COLUMN UserPassword NVARCHAR(512);

ALTER TABLE Users
ADD Role int NOT NULL DEFAULT(0)

ALTER TABLE Users
ADD IsActive bit NOT NULL DEFAULT(1);

UPDATE Users
SET Gender = CASE
    WHEN Gender LIKE 'Mann%' OR Gender LIKE 'Male%' THEN '1'
    WHEN Gender LIKE 'Frau%'  OR Gender LIKE 'Female%' THEN '2'
	WHEN Gender LIKE 'Other%' OR Gender LIKE 'Diverse%' THEN '3'
    ELSE NULL
END;

ALTER TABLE Users
ALTER COLUMN Gender INT
";
}