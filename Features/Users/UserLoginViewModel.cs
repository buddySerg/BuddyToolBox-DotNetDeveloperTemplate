using BTB.Core.Base;
using BTB.Core.Enums;
using BTB.Core.Localization;
using BTB.Data;
using BTB.Data.Entities;
using BTB.Data.Repositories;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.Extensions.Logging;

namespace BTB.Features.Users;

public partial class UserLoginViewModel : LocalizedViewModelBase
{

    #region private Fields
    private readonly ILogger<UserLoginViewModel> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ApplicationState _applicationState;
    #endregion

    #region Public Properties
    [ObservableProperty]
    private string _userName;
    [ObservableProperty]
    private string _userPassword = string.Empty;

    [ObservableProperty]
    private string _loginMessage = string.Empty;
    #endregion

    // Constructor
    public UserLoginViewModel(ILocalizationService localization, IUserRepository userRepository, ILogger<UserLoginViewModel> logger, ApplicationState applicationState) : base(localization)
    {
        _userRepository = userRepository;
        _logger = logger;
        _applicationState = applicationState;
    }

    #region Public Methods
    /// <summary>
    /// Validates the login
    /// </summary>
    public async Task<bool> ValidateLoginAsync()
    {
        LoginMessage = string.Empty;

        Result<User> loginResult = await _userRepository.LoginAsync(UserName, UserPassword);

        if (loginResult.Status == ResultStatus.Unauthorized)
        {
            LoginMessage = loginResult.Message;
            return false;
        }

        if (loginResult.IsSuccess)
        {
            _applicationState.SetCurrentUser(loginResult.Value);
            _logger.LogInformation("User {UserName} logged in successfully.", UserName);
            return true;
        }
        else
        {
            LoginMessage = loginResult.Message;
            return false;
        }
    }
    #endregion

    #region Events
    #endregion

    #region Event Subscription
    #endregion

    #region Commands
    #endregion

    #region Private Methods
    #endregion

}
