using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Core.Navigation;

/// <summary>
/// Tracks the current navigation entry and supports backward navigation.
/// </summary>
public sealed class NavigationState
{

    #region Private Fields
    private readonly Stack<NavigationEntry> _backStack = new();
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the currently active navigation entry.
    /// </summary>
    public NavigationEntry? Current { get; private set; }

    /// <summary>
    /// Gets the current entry view model as an observable object.
    /// </summary>
    public ObservableObject? CurrentViewModel => (ObservableObject?)(Current?.ViewModel);

    /// <summary>
    /// Gets the route of the current navigation entry.
    /// </summary>
    public string? CurrentRoute => Current?.Context.Route;

    /// <summary>
    /// Gets a value indicating whether at least one previous entry exists.
    /// </summary>
    public bool CanGoBack => _backStack.Count > 0;
    #endregion

    #region Events
    /// <summary>
    /// Raised whenever the current navigation entry changes.
    /// </summary>
    public event EventHandler<NavigationEntry?>? CurrentChanged;
    /// <summary>
    /// Raises the current-changed notification with the latest current entry.
    /// </summary>
    private void RaiseCurrentChanged() => CurrentChanged?.Invoke(this, Current);
    #endregion

    #region Public Methods
    /// <summary>
    /// Navigates to a new entry and pushes the existing current entry onto the back stack.
    /// </summary>
    public void Navigate(NavigationEntry navigation)
    {
        ArgumentNullException.ThrowIfNull(navigation);

        if (Current is not null)
        {
            _backStack.Push(Current);
        }

        Current = navigation;

        RaiseCurrentChanged();
    }

    /// <summary>
    /// Restores the previous entry from the back stack and makes it current.
    /// </summary>
    public NavigationEntry? GoBack()
    {
        if (!_backStack.TryPop(out NavigationEntry? previous))
        {
            return null;
        }

        Current = previous;

        RaiseCurrentChanged();

        return previous;
    }

    /// <summary>
    /// Clears the full navigation state and resets the current entry.
    /// </summary>
    public void Clear()
    {
        _backStack.Clear();
        Current = null;

        RaiseCurrentChanged();
    }
    #endregion
}