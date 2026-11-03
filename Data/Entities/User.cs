using System.ComponentModel.DataAnnotations.Schema;

namespace BTB.Data.Entities;

public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Gender { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Initials { get; set; }
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? UserName { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();

    [NotMapped]
    public string Address => string.Join(", ", new[] { Street, $"{ZipCode} {City}".Trim() }.Where(x => !string.IsNullOrWhiteSpace(x)));

    [NotMapped]
    public string Location => string.Join(", ", new[] { City, Country }.Where(x => !string.IsNullOrWhiteSpace(x)));
}
