using Microsoft.VisualBasic;

namespace TestAssignmentWebAPI.Entities;

public class Task
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; }
    
    public enum Status
    {
        Pending = 1,
        InProgess = 2,
        Completed = 3
    }

    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

}