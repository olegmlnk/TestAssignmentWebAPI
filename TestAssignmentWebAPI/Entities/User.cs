using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace TestAssignmentWebAPI.Entities;

//Class representing a user in the system, with properties for ID, username, email, password hash,
//creation and update timestamps, and a collection of associated tasks.
//I use data annotations to enforce validation rules on properties like Username, Email, and PasswordHash.
//In another case I would use an Identity framework for user management, e.g., for password requirements.
public class User
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow ;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Task> Tasks { get;  set; }
}