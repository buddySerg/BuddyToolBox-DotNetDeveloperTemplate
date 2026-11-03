using BTB.Core.Base;
using BTB.Core.Dialogs;
using BTB.Core.Localization;
using BTB.Core.Navigation;
using BTB.Core.Settings;
using BTB.Data;
using BTB.Data.Entities;
using BTB.Data.Repositories;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BTB.Features.Users
{
    /// <summary>
    /// Provides user list management, selection handling, navigation actions, and delete workflows for the users view.
    /// </summary>
    public partial class UsersViewModel : LocalizedViewModelBase, INavigationAware
    {
        #region Private Fields
        private readonly IUserRepository _repository;
        private readonly ILogger<UsersViewModel> _logger;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private bool CanEdit() => SelectedUserCount == 1;
        private bool CanDelete() => SelectedUserCount > 0;
        private bool CanRefresh() => !IsBusy;
        #endregion

        #region Observable Properties
        /// <summary>
        /// Stores the users displayed in the view.
        /// </summary>
        [ObservableProperty]
        private RangeObservableCollection<UserViewModel> _users = new RangeObservableCollection<UserViewModel>();

        /// <summary>
        /// Tracks how many users are currently selected and updates command availability.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
        private int _selectedUserCount;

        /// <summary>
        /// Indicates whether a load operation is currently running.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        private bool _isBusy;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the users view model.
        /// </summary>
        public UsersViewModel(IOptionsMonitor<ApplicationSettings> settings, ILocalizationService localization, IUserRepository repository, ILogger<UsersViewModel> logger, INavigationService navigationService, IDialogService dialogService)
        : base(localization)
        {
            _repository = repository;
            _logger = logger;
            _navigationService = navigationService;
            _dialogService = dialogService;
        }
        #endregion

        #region Navigation
        /// <summary>
        /// Loads users when navigation enters this view.
        /// </summary>
        public async Task OnNavigatedToAsync(RouteContext context, CancellationToken cancellationToken = default)
        {
            await LoadAsync(cancellationToken);
        }

        /// <summary>
        /// Releases user subscriptions when navigation leaves this view.
        /// </summary>
        public Task OnNavigatedFromAsync(RouteContext context, CancellationToken cancellationToken = default)
        {
            UnsubscribeUsers();
            _logger.LogDebug("Users view deactivated.");
            return Task.CompletedTask;
        }
        #endregion

        #region Events
        /// <summary>
        /// Updates the selected user counter when a user's selection state changes.
        /// </summary>
        private void User_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(UserViewModel.IsSelected))
            {
                return;
            }

            SelectedUserCount = Users.Count(x => x.IsSelected);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Loads all users from the repository and refreshes the list in the view.
        /// </summary>
        [RelayCommand]
        private async Task LoadAsync(CancellationToken cancellationToken)
        {
            if (IsBusy)
            {
                _logger.LogDebug("User loading skipped because another load is running.");
                return;
            }

            try
            {
                IsBusy = true;

                Result<List<User>> result = await _repository.GetAllAsync(cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to load users: {Error}", result.Message);
                    await _dialogService.ShowError(Translate["Features.Users.LoadUsersFailed"]);
                    return;
                }

                if (!result.TryGetValue(out List<User>? loadedUsers) || loadedUsers is null)
                {
                    _logger.LogWarning("Failed to load users: result contains no value.");
                    await _dialogService.ShowError(Translate["Features.Users.LoadUsersFailed"]);
                    return;
                }

                UnsubscribeUsers();

                List<UserViewModel> users = [.. loadedUsers
                        .Select(user =>
                        {
                            UserViewModel vm = UserViewModel.FromEntity(user);
                            vm.PropertyChanged += User_PropertyChanged;
                            return vm;
                        })];

                Users.ClearAndAddRange(users);

                SelectedUserCount = Users.Count(x => x.IsSelected);

                _logger.LogDebug("Successfully loaded {Count} users.", Users.Count);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Loading users was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading users.");

                await _dialogService.ShowError(Translate["Features.Users.LoadUsersFailed"]);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Navigates to the user creation view.
        /// </summary>
        [RelayCommand]
        private async Task AddAsync()
        {
            try
            {
                await _navigationService.NavigateAsync(UsersFeatureExtensions.UsersCreateRoute);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to navigate to create user.");
                await _dialogService.ShowError(Translate["Features.Users.NavigationFailed"]);
            }
        }

        /// <summary>
        /// Navigates to the edit view for the currently selected user.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanEdit))]
        private async Task EditAsync()
        {
            UserViewModel? selectedUser = Users.SingleOrDefault(x => x.IsSelected);

            if (selectedUser is null)
            {
                _logger.LogWarning("Edit requested without exactly one selected user.");
                return;
            }

            try
            {
                _logger.LogDebug("Navigating to edit user {UserId}.", selectedUser.UserId);
                await _navigationService.NavigateAsync($"users/edit/{selectedUser.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to navigate to edit user {UserId}.", selectedUser.UserId);
                await _dialogService.ShowError(Translate["Features.Users.NavigationFailed"]);
            }
        }

        /// <summary>
        /// Reloads the user list.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanRefresh))]
        private async Task RefreshAsync()
        {
            await LoadAsync(cancellationToken: CancellationToken.None);
        }

        /// <summary>
        /// Deletes all currently selected users after confirmation and reports partial failures.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteAsync()
        {
            List<UserViewModel> selectedUsers = Users.Where(x => x.IsSelected).ToList();

            bool confirmed = await _dialogService.Confirm(Translate["Features.Users.DeleteUsersConfirmMessage"], Translate["Features.Users.DeleteUsersConfirmTitle"]);

            if (!confirmed)
            {
                return;
            }

            List<UserViewModel> deletedUsers = [];
            List<Guid> failedUserIds = [];

            foreach (UserViewModel user in selectedUsers)
            {
                try
                {
                    Result result = await _repository.DeleteAsync(user.UserId);

                    if (result.IsSuccess)
                    {
                        user.PropertyChanged -= User_PropertyChanged;
                        deletedUsers.Add(user);

                        _logger.LogInformation("User {UserId} deleted successfully.", user.UserId);
                    }
                    else
                    {
                        failedUserIds.Add(user.UserId);

                        _logger.LogWarning("Failed to delete user {UserId}: {Error}", user.UserId, result.Message);
                    }
                }
                catch (Exception ex)
                {
                    failedUserIds.Add(user.UserId);
                    _logger.LogError(ex, "Unexpected error while deleting user {UserId}.", user.UserId);
                }
            }

            if (deletedUsers.Count > 0)
            {
                Users.RemoveRange(deletedUsers);
                SelectedUserCount = Users.Count(x => x.IsSelected);
            }

            if (failedUserIds.Count > 0)
            {
                _logger.LogWarning("Deletion completed with {SuccessCount} successes and {FailureCount} failures.", deletedUsers.Count, failedUserIds.Count);

                await _dialogService.ShowError(Translate["Features.Users.DeleteUsersFailed"]);
            }
            else if (deletedUsers.Count > 0)
            {
                await _dialogService.ShowInfo(Translate["Features.Users.DeleteUsersSucceeded"]);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Removes selection change subscriptions from all current user view models.
        /// </summary>
        private void UnsubscribeUsers()
        {
            foreach (UserViewModel user in Users)
            {
                user.PropertyChanged -= User_PropertyChanged;
            }
        }
        #endregion
    }
}
