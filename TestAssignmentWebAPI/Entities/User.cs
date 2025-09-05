using System.Runtime.InteropServices.JavaScript;

namespace TestAssignmentWebAPI.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow ;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public ICollection<Task> Tasks { get; private set; }
}