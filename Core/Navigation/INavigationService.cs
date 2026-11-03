namespace BTB.Core.Navigation;

/// <summary>
/// Defines navigation operations for moving between application routes.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Indicates whether the navigation service can navigate back to the previous page.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Navigates to the specified route asynchronously.
    /// </summary>
    Task NavigateAsync(string route, CancellationToken cancellationToken = default);

    /// <summary>
    /// Navigates back to the previous page asynchronously.
    /// </summary>
    Task GoBackAsync(CancellationToken cancellationToken = default);
}