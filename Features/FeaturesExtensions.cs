using BTB.Core.Navigation;
using BTB.Features.Settings;
using BTB.Features.Users;

using Microsoft.Extensions.DependencyInjection;

namespace BTB.Features
{
    /// <summary>
    /// Provides extension methods for registering feature modules.
    /// </summary>
    public static class FeaturesExtensions
    {
        /// <summary>
        /// Registers all feature services and shared navigation configuration.
        /// </summary>
        public static IServiceCollection AddFeatures(this IServiceCollection services)
        {
            NavigationRegistry navigation = new();

            // Features
            services
                .AddUsersFeature(navigation)
                .AddSettingsFeature(navigation);

            // Navigation
            services.AddSingleton(navigation);

            return services;
        }
    }
}