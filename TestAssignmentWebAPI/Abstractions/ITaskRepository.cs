namespace TestAssignmentWebAPI.Abstractions;

public interface ITaskRepository
{
    Task<Entities.Task?> GetTaskByIdAsync(Guid? id);
    Task<IEnumerable<Entities.Task>> GetAllTasksAsync();
    Task<Entities.Task> AddTaskAsync(Entities.Task task);
    Task UpdateTaskAsync(Entities.Task? task);
    Task DeleteTaskAsync(Guid? id);
}