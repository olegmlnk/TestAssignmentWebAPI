using TestAssignmentWebAPI.Entities;

namespace TestAssignmentWebAPI.Contracts.TaskDtos;

public class FilterTaskDto
{
    public Status? Status { get; set; }
    public Priority? Priority { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public string? SortBy { get; set; } = "CreatedAt"; // We will sorting by CreatedAt, DueDate, Priority and Status, but defualt is CreatedAt
    public string? SortOrder { get; set; } = "desc"; // asc or desc but default is desc
    public int PageNumber { get; set; } = 1; // A default page number is 1
    public int PageSize { get; set; } 
}