using System.ComponentModel.DataAnnotations;

namespace TestAssignmentWebAPI.Contracts.TaskDtos;

public record CreateTaskDto
(
    string Title,
    string Description,
    DateTime DueDate,
    string Priority,
    string Status
);