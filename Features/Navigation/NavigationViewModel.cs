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
    private readonly NavigationState _navigationState;
    private readonly Dictionary<NavigationViewItem, NavigationRegistration> _items = new();
    #region Public properties
    public object? CurrentViewModel => _navigationState.CurrentViewModel;
    public string? CurrentRoute => _navigationState.CurrentRoute;
    public bool CanGoBack => _navigationState.CanGoBack;
    #endregion

    #region Observable collections
    public ObservableCollection<NavigationViewItem> NavigationMenuItems { get; }
    public ObservableCollection<NavigationViewItem> NavigationFooterItems { get; }
    #endregion

    /// <summary>
    /// Constructor for the NavigationViewModel class.
    /// </summary>
    public NavigationViewModel(
        ILocalizationService localizationService,
        NavigationState navigationState,
        NavigationRegistry registry,
        INavigationService navigationService) : base(localizationService)
    {
        this._navigationState = navigationState;

        NavigationMenuItems = new ObservableCollection<NavigationViewItem>(
            registry.GetMenuItems().Select(item => CreateNavigationItem(item, navigationService)));

        NavigationFooterItems = new ObservableCollection<NavigationViewItem>(
            registry.GetFooterItems().Select(item => CreateNavigationItem(item, navigationService)));

        navigationState.CurrentChanged += OnCurrentChanged;
    }

    protected override void OnCultureChanged()
    {
        base.OnCultureChanged();

        foreach (var pair in _items)
        {
            pair.Key.Content = Translate[pair.Value.Title];
        }
    }

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

    private NavigationViewItem CreateNavigationItem(NavigationRegistration item, INavigationService navigationService)
    {
        NavigationViewItem navigationItem = new NavigationViewItem
        {
            Content = Translate[item.Title],
            Tag = item.Route,
            Command = new AsyncRelayCommand(() => navigationService.NavigateAsync(item.Route)),
        };

        // Create IconControl
        if (!string.IsNullOrWhiteSpace(item.IconResourceKey))
        {
            navigationItem.Icon = new IconControl
            {
                IconResourceKey = item.IconResourceKey,
                IconSize = 24
            };
        }

        _items[navigationItem] = item;

        return navigationItem;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _navigationState.CurrentChanged -= OnCurrentChanged;
        }

        base.Dispose(disposing);
    }
}