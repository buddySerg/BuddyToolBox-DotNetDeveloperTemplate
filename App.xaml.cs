using System.Globalization;
using System.IO;
using System.Windows;

using BTB.Core;
using BTB.Core.Dialogs;
using BTB.Core.Localization;
using BTB.Core.Navigation;
using BTB.Core.Repositories;
using BTB.Core.Security;
using BTB.Core.Settings;
using BTB.Core.Theme;
using BTB.Data;
using BTB.Data.Database;
using BTB.Features;
using BTB.Features.Navigation;
using BTB.Features.SplashScreen;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Serilog;

namespace BTB;

public partial class App : Application
{
    // private Properties
    private IHost? _host;
    private const int SplashScreenDelay = 3000; // Minimum time to show the splash screen in milliseconds

    private static IHost CreateHost()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        ConfigureConfigurationAndSettings(builder);
        ConfigureLogging(builder);
        ConfigureServices(builder);
        ConfigureDatabase(builder);

        return builder.Build();
    }

    private static void ConfigureConfigurationAndSettings(HostApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true);

        // Application Settings ( appsettings.json in the root of the project )
        builder.Services.AddOptions<ApplicationSettings>()
            .Bind(builder.Configuration)
            .Validate(settings => settings.AppSettings.MaxItems is >= 1 and <= 10000, "ApplicationSettings: MaxItems must be between 1 and 10000.")
            .ValidateOnStart();
    }

    private static void ConfigureLogging(HostApplicationBuilder builder)
    {
        builder.Services
            .AddSerilog((services, configuration) => configuration.ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services));
    }


    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static void ConfigureServices(HostApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;

        // Register JsonRepository for UserSettings, ensuring that the settings file is created in the user's local application data directory.
        services.AddSingleton<IJsonRepository<UserSettings>>(sp =>
        {
            ILogger<JsonRepository<UserSettings>> logger = sp.GetRequiredService<ILogger<JsonRepository<UserSettings>>>();
            string settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BuddyToolBox");
            Directory.CreateDirectory(settingsDirectory);
            return new JsonRepository<UserSettings>(Path.Combine(settingsDirectory, "settings.user.json"), logger);
        });
        services.AddSingleton<IUserSettingsService, UserSettingsService>();

        // DPAPI (Data Protection API)
        services.AddSingleton<IEncryptionService, DpapiEncryptionService>();

        // Database
        services.AddTransient<DatabaseUpdateService>();
        services.AddSingleton<IConnectionStringFactory, SqlConnectionStringFactory>();

        // Localization
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<LocalizationViewModel>();

        // Theme
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<ThemeViewModel>();

        // Dialogs
        services.AddSingleton<IDialogService, DialogService>();

        // Navigation
        services.AddNavigation();

        // SplashScreen
        services.AddSingleton<SplashViewModel>();
        services.AddTransient<SplashWindow>();

        // UI
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        // Features
        services.AddFeatures();
    }

    private static void ConfigureDatabase(HostApplicationBuilder builder)
    {
        builder.Services.AddDbContextFactory<AppDbContext>(
              (sp, options) =>
              {
                  IOptionsMonitor<ApplicationSettings> settingsMonitor = sp.GetRequiredService<IOptionsMonitor<ApplicationSettings>>();
                  IConnectionStringFactory factory = sp.GetRequiredService<IConnectionStringFactory>();
                  string connectionString = factory.Create(settingsMonitor.CurrentValue.Sql);

                  options.UseSqlServer(connectionString, sql =>
                      {
                          sql.EnableRetryOnFailure(
                              maxRetryCount: 5,
                              maxRetryDelay: TimeSpan.FromSeconds(10),
                              errorNumbersToAdd: null);
                          sql.CommandTimeout(30);
                      });
#if DEBUG
                  options.EnableDetailedErrors();
                  options.UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>());
                  options.EnableSensitiveDataLogging();
#endif
              });
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        SplashWindow? splash = null;

        try
        {
            // Create and start the host
            _host = CreateHost();
            await _host.StartAsync();

            // Show Splash Screen
            SplashViewModel splashViewModel = _host.Services.GetRequiredService<SplashViewModel>();
            splash = _host.Services.GetRequiredService<SplashWindow>();
            splash.Show();

            // Settings 
            await splashViewModel.UpdateStatusAsync(StartupStep.LoadingSettings, 15);
            IUserSettingsService userSettingsService = _host.Services.GetRequiredService<IUserSettingsService>();
            await userSettingsService.LoadAsync();

            // Theme
            await splashViewModel.UpdateStatusAsync(StartupStep.ApplyingTheme, 30);
            IThemeService themeService = _host.Services.GetRequiredService<IThemeService>();
            themeService.SetTheme(userSettingsService.CurrentSettings.Theme);

            // Database migrations
            try
            {
                await splashViewModel.UpdateStatusAsync(StartupStep.UpdatingDatabase, 55);
                DatabaseUpdateService databaseUpdateService = _host.Services.GetRequiredService<DatabaseUpdateService>();
                await databaseUpdateService.UpdateAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Database connection failed.");
                MessageBox.Show("Database connection failed.\n\nSee log file for details.", "Database connection error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // localization

            await splashViewModel.UpdateStatusAsync(StartupStep.ApplyingLanguage, 70);
            ILocalizationService localizationService = _host.Services.GetRequiredService<ILocalizationService>();
            localizationService.SetCulture(CultureInfo.GetCultureInfo(userSettingsService.CurrentSettings.UiCulture));

            // Set Navigation to the Users as start
            await splashViewModel.UpdateStatusAsync(StartupStep.LoadingUsers, 85);
            INavigationService navigationService = _host.Services.GetRequiredService<INavigationService>();
            await navigationService.NavigateAsync("users");

            // MainWindow and MainViewModel
            await splashViewModel.UpdateStatusAsync(StartupStep.StartingApplication, 100);
            MainWindow window = _host.Services.GetRequiredService<MainWindow>();
            MainWindow = window;

            // Show the main window
            window.Show();

            await Task.Delay(SplashScreenDelay);
            splash.Close();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed.");
            MessageBox.Show("Application startup failed.\n\nSee log file for details.", "Application startup error", MessageBoxButton.OK, MessageBoxImage.Error);
            splash?.Close();
            Shutdown(-1);
        }
    }

    /// <summary>
    /// Stops and disposes the application host, flushes pending log entries, and completes the WPF application shutdown process.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            try
            {
                _host.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Application host shutdown failed.");
            }
            finally
            {
                _host.Dispose();
            }
        }

        Log.CloseAndFlush();

        base.OnExit(e);
    }
}