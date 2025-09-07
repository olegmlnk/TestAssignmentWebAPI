using System.ComponentModel.DataAnnotations;
using TestAssignmentWebAPI.Entities;

namespace TestAssignmentWebAPI.Contracts.TaskDtos;

public class CreateTaskDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public Status Status { get; set; } = Status.Pending;

    public Priority Priority { get; set; } = Priority.Medium;
}