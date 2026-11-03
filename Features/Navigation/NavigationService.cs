using BTB.Core.Navigation;

using Microsoft.Extensions.DependencyInjection;

namespace BTB.Features.Navigation;

public sealed class NavigationService : INavigationService, IDisposable
{
    private readonly IServiceProvider _services;
    private readonly NavigationRegistry _registry;
    private readonly NavigationState _state;
    private readonly SemaphoreSlim _navigationLock = new(1, 1);

    public bool CanGoBack => _state.CanGoBack;

    public NavigationService(
        IServiceProvider services,
        NavigationRegistry registry,
        NavigationState state)
    {
        _services = services;
        _registry = registry;
        _state = state;
    }

    public async Task NavigateAsync(string route, CancellationToken cancellationToken = default)
    {
        await _navigationLock.WaitAsync(cancellationToken);

        try
        {
            RouteContext context = _registry.Resolve(route);

            if (_state.Current?.Context == context)
            {
                return;
            }

            if (_state.Current?.ViewModel is INavigationAware current)
            {
                await current.OnNavigatedFromAsync(_state.Current.Context, cancellationToken);
            }

            object viewModel = _services.GetRequiredService(context.ViewModelType!);

            if (viewModel is INavigationAware target)
            {
                await target.OnNavigatedToAsync(context, cancellationToken);
            }

            _state.Navigate(new NavigationEntry(context, viewModel));
        }
        finally
        {
            _navigationLock.Release();
        }
    }

    public async Task GoBackAsync(CancellationToken cancellationToken = default)
    {
        await _navigationLock.WaitAsync(cancellationToken);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            NavigationEntry? current = _state.Current;

            if (current is null || !_state.CanGoBack)
            {
                return;
            }

            if (current.ViewModel is INavigationAware currentAware)
            {
                await currentAware.OnNavigatedFromAsync(current.Context, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();

            NavigationEntry? previous = _state.GoBack();

            cancellationToken.ThrowIfCancellationRequested();

            if (previous?.ViewModel is INavigationAware previousAware)
            {
                await previousAware.OnNavigatedToAsync(previous.Context, cancellationToken);
            }
        }
        finally
        {
            _navigationLock.Release();
        }
    }

    public void Dispose()
    {
        _navigationLock.Dispose();
    }
}