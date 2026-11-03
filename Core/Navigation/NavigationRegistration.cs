namespace BTB.Core.Navigation;

/// <summary>
/// Represents a navigation entry used to register routable views and menu metadata.
/// </summary>
public sealed record NavigationRegistration
{
    /// <summary>
    /// Gets the route used for navigation.
    /// </summary>
    public required string Route { get; init; }

    /// <summary>
    /// Gets the view model type associated with the route.
    /// </summary>
    public required Type ViewModelType { get; init; }

    /// <summary>
    /// Gets the display title for the navigation entry.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the optional resource key for the entry icon.
    /// </summary>
    public string? IconResourceKey { get; init; }

    /// <summary>
    /// Gets the display order of the navigation entry.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Gets a value indicating whether the entry is shown in the main menu.
    /// </summary>
    public bool IsMenuItem { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the entry is shown in the footer area.
    /// </summary>
    public bool IsFooterItem { get; init; }
}