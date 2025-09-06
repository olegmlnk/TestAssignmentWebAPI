using TestAssignmentWebAPI.Contracts.TaskDtos;

namespace TestAssignmentWebAPI.Abstractions;

public interface ITaskService
{
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto taskDto, Guid userId);
    Task<TaskResponseDto> GetTaskByIdAsync(Guid id, Guid userId);
    Task<PaginationResultDto<TaskResponseDto>> GetAllTasksAsync(Guid userId, FilterTaskDto filter);
    Task<TaskResponseDto?> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto, Guid userId);
    Task<bool> DeleteTaskAsync(Guid id, Guid userId);
}