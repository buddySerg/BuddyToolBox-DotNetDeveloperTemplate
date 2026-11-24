# Build Instructions

## Prerequisites

Before building the application, ensure the following software is installed:
- .NET SDK 8.0 or later
- Visual Studio
- SQL Server (if required by the application)

## Install Microsoft Extensions
```bash
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
```

## Install Serilog Extensions
```bash
dotnet add package Serilog
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Extensions.Hosting
dotnet add package Serilog.Settings.Configuration
```

## Getting Started
1. Clone the repository.
2. Open the solution in Visual Studio.
3. Restore NuGet packages.
4. Update the SQL database settings in `appsettings.json`.
```json
"Sql": {
    "Server": "localhost\\SQLEXPRESS",
    "Database": "myDatabase",
    "UseIntegratedSecurity": true,
    "TrustServerCertificate": true,
    "Encrypt": true
  }
```
5. Update the logging settings in `appsettings.json`.

   The application uses **Serilog** for logging.
   


```json
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Error",
        "System": "Error",
        "Microsoft.EntityFrameworkCore.Database.Command": "Error"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level}] => {Message:lj}{NewLine}{Exception}",
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithThreadId",
      "WithMachineName"
    ]
  }
```
6. Build and run the application.

## Final Notes

Thank you for using this project.

I hope this project helps you build great applications. Have fun with it! 🚀

