using System.ComponentModel;

using BTB.Core.Base;
using BTB.Core.Dialogs;
using BTB.Core.Helpers;
using BTB.Core.Localization;
using BTB.Core.Navigation;
using BTB.Data;
using BTB.Data.Entities;
using BTB.Data.Repositories;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Logging;

namespace BTB.Features.Users;

/// <summary>
/// Provides create and edit behavior for a single user.
/// </summary>
public partial class UserEditViewModel : LocalizedViewModelBase, INavigationAware
{
    #region Private Fields
    private readonly IUserRepository _repository;
    private readonly INavigationService _navigationService;
    private readonly ILogger<UserEditViewModel> _logger;
    private readonly IDialogService _dialogService;
    private User? _snapshot;
    private bool CanSave => IsDirty;
    #endregion

    #region Observable Properties
    [ObservableProperty]
    private UserViewModel? user;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private bool isEditMode;

    [ObservableProperty]
    private bool isDirty;

    [ObservableProperty]
    private Guid? userId;
    #endregion

    /// <summary>
    /// Gets the localized title for the current editor mode.
    /// </summary>
    public string Title => IsEditMode ? Translate["Features.Users.EditUser"] : Translate["Features.Users.AddUser"];

    #region Constructor
    public UserEditViewModel(ILocalizationService localization, IUserRepository repository, INavigationService navigationService, ILogger<UserEditViewModel> logger, IDialogService dialogService) : base(localization)
    {
        _repository = repository;
        _navigationService = navigationService;
        _logger = logger;
        _dialogService = dialogService;
    }
    #endregion

    #region Navigation
    /// <summary>
    /// Loads editor state when navigation enters this view model.
    /// </summary>
    public async Task OnNavigatedToAsync(RouteContext context, CancellationToken cancellationToken = default)
    {
        IsEditMode = false;
        UserId = null;

        if (!context.Parameters.TryGetValue("UserId", out string? value))
        {
            User = new UserViewModel();
            SubscribeToUser();
            IsDirty = false;
            return;
        }

        if (!Guid.TryParse(value, out Guid outUserId))
        {
            _logger.LogWarning("Invalid UserId provided during navigation: {UserId}", value);
            await _dialogService.ShowError(Translate["Features.Users.InvalidUserId"]);
            return;
        }

        try
        {
            UserId = outUserId;
            IsEditMode = true;

            Result<User> result = await _repository.GetByIdAsync((Guid)UserId, cancellationToken);

            if (!result.IsSuccess || !result.TryGetValue(out User? loadedUser) || loadedUser is null)
            {
                _logger.LogWarning("Failed to load user {UserId}. Error: {Error}", UserId, result.Message);
                await _dialogService.ShowError(Translate["Features.Users.LoadUserFailed"]);
                return;
            }

            User = UserViewModel.FromEntity(loadedUser);
            _snapshot = loadedUser;
            IsDirty = false;
            SubscribeToUser();
            _logger.LogDebug("User {UserId} loaded successfully.", UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading user {UserId}.", UserId);
            await _dialogService.ShowError(Translate["Features.Users.LoadUserFailed"]);
        }
    }

    /// <summary>
    /// Cleans up subscriptions when navigation leaves this view model.
    /// </summary>
    public Task OnNavigatedFromAsync(RouteContext context, CancellationToken cancellationToken = default)
    {
        UnsubscribeFromUser();
        return Task.CompletedTask;
    }
    #endregion

    #region Events
    /// <summary>
    /// Tracks changes in the editable user model and updates save availability.
    /// </summary>
    private void OnUserPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (User is null)
        {
            return;
        }

        if (_snapshot is null)
        {
            IsDirty = true;
        }
        else
        {
            IsDirty = !ObjectComparer.AreEqual(_snapshot, User.ToEntity());
        }
        SaveCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Commands
    /// <summary>
    /// Persists the current user and leaves the editor when saving succeeds.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (User is null)
        {
            _logger.LogWarning("Save requested, but no user is available.");
            return;
        }

        UserViewModel userVM = User;

        try
        {
            Result<User> result;

            if (IsEditMode)
            {
                result = await _repository.UpdateAsync(userVM.ToEntity());
            }
            else
            {
                userVM.UserId = Guid.NewGuid();
                result = await _repository.AddAsync(userVM.ToEntity());
            }

            if (!result.IsSuccess || !result.TryGetValue(out User? savedUser) || savedUser is null)
            {
                _logger.LogWarning("Failed to {Operation} user. UserId: {UserId}", IsEditMode ? "update" : "create", userVM.UserId);
                await _dialogService.ShowError(Translate["Features.Users.SaveUserFailed"]);
                return;
            }

            userVM.UserId = savedUser.UserId;
            IsEditMode = true;
            _snapshot = savedUser;
            IsDirty = false;
            SaveCommand.NotifyCanExecuteChanged();

            _logger.LogDebug("User {UserId} saved successfully.", userVM.UserId);

            await _dialogService.ShowInfo(Translate["Features.Users.SaveUserSucceeded"]);

            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while saving user {UserId}.", userVM.UserId);
            await _dialogService.ShowError(Translate["Features.Users.SaveUserFailed"]);
        }
    }

    /// <summary>
    /// Cancels editing and optionally discards unsaved changes.
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        if (IsDirty)
        {
            bool discardChanges = await _dialogService.Confirm(Translate["Features.Users.DiscardChangesMessage"], Translate["Features.Users.DiscardChangesTitle"]);

            if (!discardChanges)
            {
                _logger.LogDebug("Cancel operation aborted because the user kept editing.");
                return;
            }
        }

        _logger.LogDebug("Leaving user editor. UserId: {UserId}", UserId);
        await _navigationService.GoBackAsync();
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Subscribes to property change notifications of the editable user model.
    /// </summary>
    private void SubscribeToUser()
    {
        if (User is null)
        {
            return;
        }
        User.PropertyChanged -= OnUserPropertyChanged;
        User.PropertyChanged += OnUserPropertyChanged;
    }

    /// <summary>
    /// Removes property change notifications of the editable user model.
    /// </summary>
    private void UnsubscribeFromUser()
    {
        if (User is null)
        {
            return;
        }

        User.PropertyChanged -= OnUserPropertyChanged;
    }
    #endregion
}