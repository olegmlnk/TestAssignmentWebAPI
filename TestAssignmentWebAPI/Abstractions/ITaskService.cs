namespace TestAssignmentWebAPI.Abstractions;

public interface ITaskService
{
    Task<Task> CreateTaskAsync(Task task);
    Task<Task> GetTaskByIdAsync(Guid id);
    Task<IEnumerable<Task>> GetAllTasksAsync();
    Task<bool> UpdateTaskAsync(Task task);
    Task<bool> DeleteTaskAsync(Guid id);
}