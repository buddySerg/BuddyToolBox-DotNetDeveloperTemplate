using System.IO;

using BTB.Core.Navigation;
using BTB.Core.Repositories;
using BTB.Core.Settings;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BTB.Features.Settings;

/// <summary>
/// Provides dependency injection and navigation registration for the settings feature.
/// </summary>
public static class SettingsFeatureExtensions
{
    /// <summary>
    /// Gets the route identifier used to access the settings feature.
    /// </summary>
    public const string Route = "settings";

    /// <summary>
    /// Registers the settings repository, views, view model, and navigation entry.
    /// </summary>
    public static IServiceCollection AddSettingsFeature(this IServiceCollection services, NavigationRegistry navigation)
    {
        // JSON repository registration
        services.AddSingleton<IJsonRepository<ApplicationSettings>>(sp =>
        {
            ILogger<JsonRepository<ApplicationSettings>> logger = sp.GetRequiredService<ILogger<JsonRepository<ApplicationSettings>>>();
            string filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            return new JsonRepository<ApplicationSettings>(filePath, logger);
        });

        // View and view model registration
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsView>();

        navigation.Register<SettingsViewModel>(
            title: "Features.Settings.MenuLabel",
            route: Route,
            iconResourceKey: "SettingsIcon",
            order: 0,
            isMenuItem: false,
            isFooterItem: true);

        return services;
    }
}
