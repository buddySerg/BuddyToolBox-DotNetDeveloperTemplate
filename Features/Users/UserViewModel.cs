using BTB.Data.Entities;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Features.Users;

/// <summary>
/// Represents a user-facing view model for displaying and editing user data in the UI.
/// </summary>
public partial class UserViewModel : ObservableObject
{
    #region Observable Properties
    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private Guid userId;

    [ObservableProperty]
    private string gender = string.Empty;

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
    #endregion

    /// <summary>
    /// Gets the full name composed from first name and last name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Constructor
    public UserViewModel()
    {
    }

    /// <summary>
    /// Creates a view model instance from a user entity.
    /// </summary>
    public static UserViewModel FromEntity(User user)
    {
        return new UserViewModel
        {
            UserId = user.UserId,
            Gender = user.Gender,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Initials = user.Initials,
            Street = user.Street,
            ZipCode = user.ZipCode,
            City = user.City,
            Country = user.Country,
            Email = user.Email,
            Phone = user.Phone,
            UserName = user.UserName
        };
    }

    /// <summary>
    /// Converts the current view model into a user entity.
    /// </summary>
    public User ToEntity()
    {
        return new User
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
            UserName = UserName
        };
    }
}