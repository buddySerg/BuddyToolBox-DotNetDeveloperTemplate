namespace BTB.Core.Navigation;

/// <summary>
/// Represents a navigation entry that contains the route context and the associated view model.
/// </summary>
public sealed record NavigationEntry(RouteContext Context, object ViewModel);