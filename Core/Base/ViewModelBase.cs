using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Core.Base;

/// <summary>
/// Base class for all ViewModels.
/// </summary>
public abstract class ViewModelBase : ObservableObject, IDisposable
{
    private bool _isDisposed;

    /// <summary>
    /// Releases all resources used by the current instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases managed resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed || !disposing)
        {
            return;
        }

        _isDisposed = true;
    }
}