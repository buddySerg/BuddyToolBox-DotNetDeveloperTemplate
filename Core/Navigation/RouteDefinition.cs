namespace BTB.Core.Navigation;

/// <summary>
/// Represents a navigation route definition that maps a route template to a view model type.
/// </summary>
public sealed record RouteDefinition
{
    /// <summary>
    /// Gets the route template used for navigation matching.
    /// </summary>
    public required string Template { get; init; }

    /// <summary>
    /// Gets the view model type associated with the route template.
    /// </summary>
    public required Type ViewModelType { get; init; }
}