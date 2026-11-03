using BTB.Core.Navigation;

using Microsoft.Extensions.DependencyInjection;

namespace BTB.Features.Navigation;

/// <summary>
/// Provides extension methods for registering navigation-related services.
/// </summary>
public static class NavigationFeatureExtensions
{
    /// <summary>
    /// Registers the navigation state, service, and view model in the dependency injection container.
    /// </summary>
    public static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        services.AddSingleton<NavigationState>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<NavigationViewModel>();

        return services;
    }
}
