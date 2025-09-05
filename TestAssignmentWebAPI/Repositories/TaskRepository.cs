using Microsoft.EntityFrameworkCore;
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
    public async Task<Entities.Task?> GetTaskByIdAsync(Guid? id)
    {
        if (id is null)
            return null;
        
        return await _context.Tasks.FindAsync(id);       
    }

    public async Task<IEnumerable<Entities.Task>> GetAllTasksAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async Task<Entities.Task> AddTaskAsync(Entities.Task task)
    {
       await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateTaskAsync(Entities.Task task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(Guid? id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
        
    }
}