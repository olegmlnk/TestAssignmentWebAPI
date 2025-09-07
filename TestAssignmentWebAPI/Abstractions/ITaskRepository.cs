using TestAssignmentWebAPI.Contracts.TaskDtos;

namespace TestAssignmentWebAPI.Abstractions;

public interface ITaskRepository
{
    Task<Entities.Task?> GetTaskByIdAsync(Guid? id, Guid userId);
    Task<PaginationResultDto<Entities.Task>> GetTasksAsync(Guid userId, FilterTaskDto filterTaskDto);
    Task<IEnumerable<Entities.Task>> GetAllTasksAsync();
    Task<Entities.Task> AddTaskAsync(Entities.Task task);
    Task<Entities.Task> UpdateTaskAsync(Entities.Task? task);
    Task DeleteTaskAsync(Guid? id, Guid userId);
    Task<bool> TaskExistsAsync(Guid? id, Guid userId);
}