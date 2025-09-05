using TestAssignmentWebAPI.Abstractions;

namespace TestAssignmentWebAPI.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    
    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    
    // Ensure that the task belongs to the user
    //We use this method like GetTaskByIdAsync, UpdateTaskAsync and DeleteTaskAsync, where we need to check if the user owns the task
    private static void EnsureOwnership(Guid userId, Entities.Task task)
    {
        if (task.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not own this task.");
        }
    }

    public Task<Task> CreateTaskAsync(Task task)
    {
        throw new NotImplementedException();
    }

    public Task<Task> GetTaskByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Task>> GetAllTasksAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateTaskAsync(Task task)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteTaskAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}