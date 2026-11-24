using BTB.Data.Entities;

namespace BTB.Core.Base;

/// <summary>
/// Manages the application's authentication state and currently signed-in user.
/// </summary>
public sealed class ApplicationState
{

    #region Public Properties
    /// <summary>
    /// Gets or sets a value indicating whether authentication is required.
    /// </summary>
    public bool IsAuthenticationRequired { get; set; }

    /// <summary>
    /// Gets the currently signed-in user, if any.
    /// </summary>
    public User? CurrentUser { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a user is currently signed in.
    /// </summary>
    public bool IsLoggedIn => CurrentUser != null;

    /// <summary>
    /// Gets a value indicating whether the application can be accessed.
    /// </summary>
    public bool CanAccessApplication => !IsAuthenticationRequired || IsLoggedIn;
    #endregion

    #region Public Methods
    /// <summary>
    /// Sets the currently signed-in user and notifies subscribers.
    /// </summary>
    public void SetCurrentUser(User user)
    {
        CurrentUser = user;
        CurrentUserChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Signs out the current user and notifies subscribers.
    /// </summary>
    public void Logout()
    {
        CurrentUser = null;
        CurrentUserChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when the current user changes.
    /// </summary>
    public event EventHandler? CurrentUserChanged;
    #endregion
}