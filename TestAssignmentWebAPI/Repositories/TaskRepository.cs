using Microsoft.EntityFrameworkCore;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts.TaskDtos;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;
    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Entities.Task?> GetTaskByIdAsync(Guid? id, Guid userId)
    {
        if (id is null)
            return null;
        
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);     
    }

    public async Task<PaginationResultDto<Entities.Task>> GetTasksAsync(Guid userId, FilterTaskDto filterTaskDto)
    {
        var query = _context.Tasks
            .Where(t => t.UserId == userId);
        
        // Here we apply the filters based on the FilterTaskDto properties
        if(filterTaskDto.Status.HasValue)
            query = query.Where(t => t.TaskStatus == filterTaskDto.Status.Value);
        
        if(filterTaskDto.Priority.HasValue)
            query = query.Where(t => t.TaskPriority == filterTaskDto.Priority.Value);
        
        if(filterTaskDto.DueDateFrom.HasValue)
            query = query.Where(t => t.DueDate >= filterTaskDto.DueDateFrom.Value);
        
        if(filterTaskDto.DueDateTo.HasValue)
            query = query.Where(t => t.DueDate <= filterTaskDto.DueDateTo.Value);

        
        // Here we apply sorting based on the SortBy and SortOrder properties
        query = filterTaskDto.SortBy?.ToLower() switch
        {
            "title" => filterTaskDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),
            "duedate" => filterTaskDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate),
            "priority" => filterTaskDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.TaskPriority)
                : query.OrderBy(t => t.TaskPriority),
            "status" => filterTaskDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.TaskStatus)
                : query.OrderBy(t => t.TaskStatus),
            _ => filterTaskDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt)
        };
        
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((filterTaskDto.PageNumber - 1) * filterTaskDto.PageSize)
            .Take(filterTaskDto.PageSize)
            .ToListAsync();

        return new PaginationResultDto<Entities.Task>()
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filterTaskDto.PageNumber,
            PageSize = filterTaskDto.PageSize
        };
    }

    public async Task<IEnumerable<Entities.Task>> GetAllTasksAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async Task<Entities.Task> AddTaskAsync(Entities.Task task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        
       await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<Entities.Task> UpdateTaskAsync(Entities.Task? task)
    {
        task.UpdatedAt = DateTime.UtcNow;
        
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteTaskAsync(Guid? id, Guid userId)
    {
        var task = await GetTaskByIdAsync(id, userId);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<bool> TaskExistsAsync(Guid? id, Guid userId)
    {
        return await _context.Tasks.AnyAsync(t => t.Id == id && t.UserId == userId);
    }
}