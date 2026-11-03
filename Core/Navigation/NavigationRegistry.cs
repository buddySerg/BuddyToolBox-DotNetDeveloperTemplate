using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Core.Navigation;

/// <summary>
/// Stores navigation route registrations and resolves runtime routes to route contexts.
/// </summary>
public sealed class NavigationRegistry
{
    #region Private Fields
    private readonly List<NavigationRegistration> _registrations = new List<NavigationRegistration>();
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets all registered navigation entries.
    /// </summary>
    public IReadOnlyCollection<NavigationRegistration> Registrations => _registrations;
    #endregion

    #region Public Methods
    /// <summary>
    /// Registers a view model type for a route template and related navigation metadata.
    /// </summary>
    public void Register<TViewModel>(
        string route,
        string title,
        string? iconResourceKey = null,
        int order = 0,
        bool isMenuItem = true,
        bool isFooterItem = false)
        where TViewModel : ObservableObject
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);

        if (_registrations.Any(x => string.Equals(x.Route, route, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Route '{route}' already registered.");
        }

        _registrations.Add(new NavigationRegistration
        {
            Route = route.Trim(),
            ViewModelType = typeof(TViewModel),
            Title = title.Trim(),
            IconResourceKey = iconResourceKey,
            Order = order,
            IsMenuItem = isMenuItem,
            IsFooterItem = isFooterItem
        });
    }

    /// <summary>
    /// Resolves a route string into a route context including path parameters and query values.
    /// </summary>
    public RouteContext Resolve(string route)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);

        string raw = route.Trim('/');

        string[] split = raw.Split('?', 2);

        string path = split[0];

        Dictionary<string, string> query =
            split.Length > 1
                ? ParseQuery(split[1])
                : new(StringComparer.OrdinalIgnoreCase);

        foreach (NavigationRegistration registration in _registrations)
        {
            if (!TryMatch(registration.Route, path, out Dictionary<string, string> parameters))
            {
                continue;
            }

            return new RouteContext
            {
                Route = path,
                Parameters = parameters,
                Query = query,
                Definition = new RouteDefinition
                {
                    Template = registration.Route,
                    ViewModelType = registration.ViewModelType
                }
            };
        }

        throw new InvalidOperationException($"Route '{route}' is not registered.");
    }

    /// <summary>
    /// Gets registered navigation items that should appear in the main menu.
    /// </summary>
    public IReadOnlyList<NavigationRegistration> GetMenuItems()
    {
        return _registrations
            .Where(x => x.IsMenuItem)
            .OrderBy(x => x.Order)
            .ToList();
    }

    /// <summary>
    /// Gets registered navigation items that should appear in the footer area.
    /// </summary>
    public IReadOnlyList<NavigationRegistration> GetFooterItems()
    {
        return _registrations
            .Where(x => x.IsFooterItem)
            .OrderBy(x => x.Order)
            .ToList();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Parses a query string into a case-insensitive key/value dictionary.
    /// </summary>
    private static Dictionary<string, string> ParseQuery(string query)
    {
        Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);

        foreach (string pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] parts = pair.Split('=', 2);

            string key = Uri.UnescapeDataString(parts[0]);

            string value =
                parts.Length > 1
                    ? Uri.UnescapeDataString(parts[1])
                    : string.Empty;

            result[key] = value;
        }

        return result;
    }

    /// <summary>
    /// Matches a concrete route against a template and extracts route parameters.
    /// </summary>
    private static bool TryMatch(string template, string route, out Dictionary<string, string> parameters)
    {
        parameters = new(StringComparer.OrdinalIgnoreCase);

        string[] templateParts = template.Trim('/').Split('/');

        string[] routeParts = route.Trim('/').Split('/');

        if (templateParts.Length != routeParts.Length)
        {
            return false;
        }

        for (int i = 0; i < templateParts.Length; i++)
        {
            string templatePart = templateParts[i];
            string routePart = routeParts[i];

            if (templatePart.StartsWith('{') && templatePart.EndsWith('}'))
            {
                string parameterName = templatePart[1..^1];
                parameters[parameterName] = Uri.UnescapeDataString(routePart);

                continue;
            }

            if (!string.Equals(templatePart, routePart, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }
        return true;
    }
    #endregion
}