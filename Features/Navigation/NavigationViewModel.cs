using System.Collections.ObjectModel;

using BTB.Core.Base;
using BTB.Core.Localization;
using BTB.Core.Navigation;
using BTB.Core.UI.Controls;

using CommunityToolkit.Mvvm.Input;

using Wpf.Ui.Controls;

namespace BTB.Features.Navigation;

public partial class NavigationViewModel : LocalizedViewModelBase
{
    #region private Fields
    private readonly NavigationState _navigationState;
    private readonly ApplicationState _applicationState;
    private readonly NavigationRegistry _navigationRegistry;
    public readonly INavigationService NavigationService;
    private readonly Dictionary<NavigationViewItem, NavigationRegistration> _items = new();
    #endregion

    #region Public properties
    public object? CurrentViewModel => _navigationState.CurrentViewModel;
    public string? CurrentRoute => _navigationState.CurrentRoute;
    public bool CanGoBack => _navigationState.CanGoBack;
    #endregion

    #region Observable collections
    public ObservableCollection<NavigationViewItem> NavigationMenuItems { get; } = new ObservableCollection<NavigationViewItem>();
    public ObservableCollection<NavigationViewItem> NavigationFooterItems { get; } = new ObservableCollection<NavigationViewItem>();
    #endregion

    /// Constructor
    public NavigationViewModel(
        ILocalizationService localizationService,
        NavigationState navigationState,
        NavigationRegistry registry,
        INavigationService navigationService,
        ApplicationState applicationState) : base(localizationService)
    {
        _navigationState = navigationState;
        _applicationState = applicationState;
        _navigationRegistry = registry;
        NavigationService = navigationService;

        LoadNavigationItems();

        _navigationState.CurrentChanged += OnCurrentChanged;
        _applicationState.CurrentUserChanged += OnCurrentUserChanged;
    }

    #region Events
    private void OnCurrentChanged(object? sender, NavigationEntry? e)
    {
        // Update active menu item
        foreach (NavigationViewItem item in NavigationMenuItems)
        {
            item.IsActive = Equals(item.Tag, CurrentRoute);
        }

        // Update active footer menu item
        foreach (NavigationViewItem item in NavigationFooterItems)
        {
            item.IsActive = Equals(item.Tag, CurrentRoute);
        }

        OnPropertyChanged(nameof(CurrentViewModel));
        OnPropertyChanged(nameof(CurrentRoute));
        OnPropertyChanged(nameof(CanGoBack));
    }

    protected override void OnCultureChanged()
    {
        base.OnCultureChanged();

        foreach (KeyValuePair<NavigationViewItem, NavigationRegistration> pair in _items)
        {
            pair.Key.Content = Translate[pair.Value.Title];
        }
    }

    private void OnCurrentUserChanged(object? sender, EventArgs e)
    {
        LoadNavigationItems();
        OnCurrentChanged(this, null);
    }
    #endregion

    #region Private Methods
    private void LoadNavigationItems()
    {
        NavigationMenuItems.Clear();
        NavigationFooterItems.Clear();
        _items.Clear();

        foreach (NavigationRegistration item in _navigationRegistry.GetMenuItems())
        {
            if (_applicationState.IsAuthenticationRequired && !item.IsVisible(_applicationState.CurrentUser))
            {
                continue;
            }
            NavigationMenuItems.Add(CreateNavigationItem(item));
        }

        foreach (NavigationRegistration item in _navigationRegistry.GetFooterItems())
        {
            if (_applicationState.IsAuthenticationRequired && !item.IsVisible(_applicationState.CurrentUser))
            {
                continue;
            }
            NavigationFooterItems.Add(CreateNavigationItem(item));
        }
    }
    private NavigationViewItem CreateNavigationItem(NavigationRegistration item)
    {
        NavigationViewItem navigationItem = new NavigationViewItem
        {
            Content = Translate[item.Title],
            Tag = item.Route,
            Command = new AsyncRelayCommand(() => NavigationService.NavigateAsync(item.Route)),
        };

        // Create IconControl
        if (!string.IsNullOrWhiteSpace(item.IconResourceKey))
        {
            navigationItem.Icon = new IconControl
            {
                IconResourceKey = item.IconResourceKey,
                IconSize = 20
            };
        }

        _items[navigationItem] = item;

        return navigationItem;
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _navigationState.CurrentChanged -= OnCurrentChanged;
            _applicationState.CurrentUserChanged -= OnCurrentUserChanged;
        }

        base.Dispose(disposing);
    }
}