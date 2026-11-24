using Microsoft.Extensions.DependencyInjection;

namespace BTB.Features.Example;

public static class ExampleFeatureExtensions
{
    public static IServiceCollection AddExampleFeature(this IServiceCollection services)
    {
        // ViewModel
        services.AddTransient<ExampleViewModel>();

        // View
        services.AddSingleton<ExampleView>();

        return services;
    }

}