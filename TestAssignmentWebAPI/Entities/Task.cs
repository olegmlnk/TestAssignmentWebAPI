using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace TestAssignmentWebAPI.Entities;
public enum Status
{
    Pending = 1,
    InProgress = 2,
    Completed = 3
}

public enum Priority
{
    Low = 1,
    Medium = 2,
    High = 3
}

public class Task
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    [MaxLength(500)]
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public Status TaskStatus { get; set; } = Status.Pending;
    public Priority TaskPriority { get; set; } = Priority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

}