using System.Runtime.InteropServices.JavaScript;

namespace TestAssignmentWebAPI.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow ;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Task> Tasks { get;  set; }
}