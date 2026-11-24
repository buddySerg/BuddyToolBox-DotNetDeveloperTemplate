using BTB.Core.Enums;
using BTB.Core.Navigation;
using BTB.Data.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace BTB.Features.Users;

/// <summary>
/// Provides dependency injection and navigation registration for the Users feature.
/// </summary>
public static class UsersFeatureExtensions
{
    /// <summary>
    /// Route for the users overview page.
    /// </summary>
    public const string UsersRoute = "users";

    /// <summary>
    /// Route for creating a new user.
    /// </summary>
    public const string UsersCreateRoute = "users/create";

    /// <summary>
    /// Route for editing an existing user.
    /// </summary>
    public const string UsersEditRoute = "users/edit/{UserId}";

    /// <summary>
    /// Registers users feature services, views, view models, and navigation entries.
    /// </summary>
    public static IServiceCollection AddUsersFeature(this IServiceCollection services, NavigationRegistry navigation)
    {
        // Repository
        services.AddTransient<IUserRepository, UserRepository>();

        // View & ViewModel
        services.AddTransient<UsersViewModel>();
        services.AddTransient<UsersView>();

        // Edit View & ViewModel
        services.AddTransient<UserEditViewModel>();
        services.AddTransient<UserEditView>();

        // Login View & ViewModel
        services.AddTransient<UserLoginViewModel>();
        services.AddTransient<UserLoginView>();

        #region Navigation Registration
        navigation.Register<UsersViewModel>(
            title: "Features.Users.MenuLabel",
            route: UsersRoute,
            iconResourceKey: "UsersIcon",
            order: 10,
            isMenuItem: true,
            isFooterItem: false,
            roles: new List<UserRole> { UserRole.Admin });

        navigation.Register<UserEditViewModel>(
            title: "Features.Users.AddUser",
            route: UsersCreateRoute,
            iconResourceKey: "UsersIcon",
            order: 0,
            isMenuItem: false,
            isFooterItem: false,
            roles: new List<UserRole> { UserRole.Admin });

        navigation.Register<UserEditViewModel>(
            title: "Features.Users.EditUser",
            route: UsersEditRoute,
            iconResourceKey: "UsersIcon",
            order: 0,
            isMenuItem: false,
            isFooterItem: false,
            roles: new List<UserRole> { UserRole.Admin });
        #endregion

        return services;
    }
}