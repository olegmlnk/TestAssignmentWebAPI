namespace TestAssignmentWebAPI.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CratedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ICollection<Task> Tasks { get; private set; }
}