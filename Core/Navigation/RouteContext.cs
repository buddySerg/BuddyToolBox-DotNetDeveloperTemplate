namespace BTB.Core.Navigation;

/// <summary>
/// Represents the resolved navigation context for a route.
/// </summary>
public sealed record RouteContext
{

    #region Public Properties
    /// <summary>
    /// Gets the route definition metadata used to resolve the context.
    /// </summary>
    public required RouteDefinition Definition { get; init; }

    /// <summary>
    /// Gets the resolved route template or path.
    /// </summary>
    public required string Route { get; init; }

    /// <summary>
    /// Gets the view-model type associated with the route definition.
    /// </summary>
    public Type ViewModelType => Definition.ViewModelType;

    /// <summary>
    /// Gets the route parameter collection.
    /// </summary>
    public IReadOnlyDictionary<string, string> Parameters { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets the query string value collection.
    /// </summary>
    public IReadOnlyDictionary<string, string> Query { get; init; } = new Dictionary<string, string>();
    #endregion

    #region Public Methods
    /// <summary>
    /// Gets a route parameter value by name, or null when not found.
    /// </summary>
    public string? GetParameter(string name)
    {
        return Parameters.GetValueOrDefault(name);
    }

    /// <summary>
    /// Gets and converts a route parameter value to the requested type.
    /// </summary>
    public T GetParameter<T>(string name)
    {
        string? value = GetParameter(name) ?? throw new KeyNotFoundException($"Parameter '{name}' not found.");

        if (typeof(T) == typeof(Guid))
        {
            return (T)(object)Guid.Parse(value);
        }

        if (typeof(T).IsEnum)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }
    #endregion
}