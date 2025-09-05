namespace TestAssignmentWebAPI.Contracts.TaskDtos;

public record UpdateTaskDto(
    string Title,
    string Description,
    DateTime DueDate,
    string Priority,
    string Status
);