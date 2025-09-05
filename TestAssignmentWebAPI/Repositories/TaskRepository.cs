using TestAssignmentWebAPI.Abstractions;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;
    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<Entities.Task> GetTaskByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Entities.Task>> GetAllTasksAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Entities.Task> AddTaskAsync(Entities.Task task)
    {
        throw new NotImplementedException();
    }

    public Task UpdateTaskAsync(Entities.Task task)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTaskAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}