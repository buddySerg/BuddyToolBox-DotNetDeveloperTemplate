using BTB.Core.Base;
using BTB.Core.Enums;
using BTB.Core.Localization;
using BTB.Data.Entities;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Features.Users;

/// <summary>
/// Represents a user-facing view model for displaying and editing user data in the UI.
/// </summary>
public partial class UserViewModel : LocalizedViewModelBase
{
    #region Private fields
    private readonly ILocalizationService _localization;
    private readonly EventHandler _cultureChangedHandler;
    #endregion

    #region Observable Properties
    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private Guid userId;

    [ObservableProperty]
    private Gender? gender;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    private string firstName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string? initials;

    [ObservableProperty]
    private string? street;

    [ObservableProperty]
    private string? zipCode;

    [ObservableProperty]
    private string? city;

    [ObservableProperty]
    private string? country;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string? phone;

    [ObservableProperty]
    private string? userName;

    [ObservableProperty]
    private UserRole role;

    [ObservableProperty]
    private bool isActive;

    [ObservableProperty]
    private string? newPassword;
    #endregion

    /// <summary>
    /// Gets the full name composed from first name and last name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string GenderDisplay => Gender is null ? string.Empty : Translate[$"Common.Gender.{Gender}"];
    public string RoleDisplay => Translate[$"Features.Users.Role.{Role}"];

    // Constructor
    public UserViewModel(User user, ILocalizationService localization) : base(localization)
    {
        _localization = localization;

        UserId = user.UserId;
        Gender = user.Gender;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Initials = user.Initials;
        Street = user.Street;
        ZipCode = user.ZipCode;
        City = user.City;
        Country = user.Country;
        Email = user.Email;
        Phone = user.Phone;
        UserName = user.UserName;
        Role = user.Role;
        IsActive = user.IsActive;

        _cultureChangedHandler = (_, _) =>
        {
            OnPropertyChanged(nameof(GenderDisplay));
            OnPropertyChanged(nameof(RoleDisplay));
        };

        _localization.CultureChanged += _cultureChangedHandler;
    }

    /// <summary>
    /// Converts the current view model into a user entity.
    /// </summary>
    public User ToEntity()
    {
        User user = new()
        {
            UserId = UserId,
            Gender = Gender,
            FirstName = FirstName,
            LastName = LastName,
            Initials = Initials,
            Street = Street,
            ZipCode = ZipCode,
            City = City,
            Country = Country,
            Email = Email,
            Phone = Phone,
            UserName = UserName,
            Role = Role,
            IsActive = IsActive
        };
        return user;
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _localization.CultureChanged -= _cultureChangedHandler;
        }

        base.Dispose(disposing);
    }
}