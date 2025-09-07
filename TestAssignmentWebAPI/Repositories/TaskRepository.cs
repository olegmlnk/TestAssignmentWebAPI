using Microsoft.EntityFrameworkCore;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts.TaskDtos;
using TestAssignmentWebAPI.Entities;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Repositories;

// The TaskRepository handles all data access logic related to tasks.
// It acts as a bridge between the business logic layer (TaskService)
// and the database context (AppDbContext).
public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<TaskRepository> _logger;

    public TaskRepository(AppDbContext context, ILogger<TaskRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Entities.Task?> GetTaskByIdAsync(Guid? id, Guid userId)
    {
        _logger.LogInformation("Retrieving task with ID {TaskId} for user {UserId}.", id, userId);
        if (id is null)
        {
            _logger.LogWarning("Attempted to retrieve task with a null ID.");
            return null;
        }
        
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);     
    }

    // Retrieves a paginated and filtered list of tasks for a specific user.
    public async Task<PaginationResultDto<Entities.Task>> GetTasksAsync(Guid userId, FilterTaskDto filterTaskDto)
    {
        _logger.LogInformation("Querying tasks for user {UserId} with filters: Status={Status}, Priority={Priority}, DueDateFrom={DueDateFrom}, DueDateTo={DueDateTo}.",
            userId, filterTaskDto.Status, filterTaskDto.Priority, filterTaskDto.DueDateFrom, filterTaskDto.DueDateTo);

        var query = _context.Tasks
            .Where(t => t.UserId == userId);
        
        // Apply filters based on the DTO properties.
        if(filterTaskDto.Status.HasValue)
            query = query.Where(t => t.TaskStatus == filterTaskDto.Status.Value);
        
        if(filterTaskDto.Priority.HasValue)
            query = query.Where(t => t.TaskPriority == filterTaskDto.Priority.Value);
        
        if(filterTaskDto.DueDateFrom.HasValue)
            query = query.Where(t => t.DueDate >= filterTaskDto.DueDateFrom.Value);
        
        if(filterTaskDto.DueDateTo.HasValue)
            query = query.Where(t => t.DueDate <= filterTaskDto.DueDateTo.Value);

        
        // Apply sorting based on the SortBy and SortOrder properties using a switch expression.
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
        _logger.LogInformation("Found a total of {TotalCount} tasks matching the criteria for user {UserId}.", totalCount, userId);

        // Apply pagination using Skip and Take.
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
        _logger.LogInformation("Retrieving all tasks from the database.");
        return await _context.Tasks.ToListAsync();
    }

    // Adds a new task to the database. It sets the new task's ID,
    // creation, and update timestamps before saving.
    public async Task<Entities.Task> AddTaskAsync(Entities.Task task)
    {
        _logger.LogInformation("Adding new task with title '{Title}' to the database.", task.Title);
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Task {TaskId} successfully added.", task.Id);
        return task;
    }

    public async Task<Entities.Task> UpdateTaskAsync(Entities.Task? task)
    {
        _logger.LogInformation("Updating task with ID {TaskId}.", task.Id);
        task.UpdatedAt = DateTime.UtcNow;
        
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Task {TaskId} successfully updated.", task.Id);
        return task;
    }

    public async Task DeleteTaskAsync(Guid? id, Guid userId)
    {
        _logger.LogInformation("Attempting to delete task with ID {TaskId} for user {UserId}.", id, userId);
        var task = await GetTaskByIdAsync(id, userId);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Task {TaskId} successfully deleted.", id);
        }
        else
        {
            _logger.LogWarning("Delete failed: Task with ID {TaskId} not found or user {UserId} does not own it.", id, userId);
        }
    }
    
    public async Task<bool> TaskExistsAsync(Guid? id, Guid userId)
    {
        return await _context.Tasks.AnyAsync(t => t.Id == id && t.UserId == userId);
    }
}
