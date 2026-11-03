namespace BTB.Core.Navigation;

/// <summary>
/// Defines lifecycle callbacks that are invoked during navigation between pages.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Invoked when navigation enters the current page.
    /// </summary>
    Task OnNavigatedToAsync(RouteContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invoked when navigation leaves the current page.
    /// </summary>
    Task OnNavigatedFromAsync(RouteContext context, CancellationToken cancellationToken = default);
}
