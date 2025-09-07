using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts.TaskDtos;
using Microsoft.Extensions.Logging; 

namespace TestAssignmentWebAPI.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskService> _logger; 
    
    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }
    
    // A private helper method to ensure a user has permission to access or modify a task.
    // This is a crucial security check to prevent users from accessing other people's data.
    private void EnsureOwnership(Guid userId, Entities.Task task)
    {
        if (task.UserId != userId)
        {
            _logger.LogWarning("Unauthorized access attempt: User {UserId} tried to access task {TaskId} owned by user {OwnerId}.", userId, task.Id, task.UserId);
            throw new UnauthorizedAccessException("User does not own this task.");
        }
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto taskDto, Guid userId)
    {
        _logger.LogInformation("Creating new task with title '{Title}' for user {UserId}.", taskDto.Title, userId);

        var task = new Entities.Task
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            TaskStatus = taskDto.Status,
            TaskPriority = taskDto.Priority,
            UserId = userId
        };
        
        var createdTask = await _taskRepository.AddTaskAsync(task);

        _logger.LogInformation("Task {TaskId} created successfully for user {UserId}.", createdTask.Id, userId);

        // Map the created entity back to a response DTO to return to the client.
        return MapToResponseDto(createdTask);
    }

    public async Task<TaskResponseDto> GetTaskByIdAsync(Guid id, Guid userId)
    {
        _logger.LogInformation("Attempting to get task {TaskId} for user {UserId}.", id, userId);

        var task = await _taskRepository.GetTaskByIdAsync(id, userId);

        if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found for user {UserId}.", id, userId);
            return null;
        }

        _logger.LogInformation("Task {TaskId} retrieved successfully.", id);
        return MapToResponseDto(task);
    }

    public async Task<PaginationResultDto<TaskResponseDto>> GetAllTasksAsync(Guid userId, FilterTaskDto filter)
    {
        _logger.LogInformation("Retrieving all tasks for user {UserId} with filters: Status={Status}, Priority={Priority}, SortBy={SortBy}, SortOrder={SortOrder}, Page={PageNumber}, PageSize={PageSize}.",
            userId, filter.Status, filter.Priority, filter.SortBy, filter.SortOrder, filter.PageNumber, filter.PageSize);
        
        // Call the repository to get the paginated and filtered tasks.
        var result = await _taskRepository.GetTasksAsync(userId, filter);

        _logger.LogInformation("Found {Count} tasks matching the criteria for user {UserId}.", result.TotalCount, userId);

        // Map the list of Task entities to a list of TaskResponseDtos.
        return new PaginationResultDto<TaskResponseDto>()
        {
            Items = result.Items.Select(MapToResponseDto),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<TaskResponseDto?> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto, Guid userId)
    {
        _logger.LogInformation("Attempting to update task {TaskId} for user {UserId}.", id, userId);
    
        var existingTask = await _taskRepository.GetTaskByIdAsync(id, userId);

        if (existingTask == null)
        {
            _logger.LogWarning("Task {TaskId} not found or user {UserId} lacks permission to update it.", id, userId);
            return null;
        }
    
        if (!string.IsNullOrWhiteSpace(updateTaskDto.Title))
            existingTask.Title = updateTaskDto.Title;

        if (!string.IsNullOrWhiteSpace(updateTaskDto.Description))
            existingTask.Description = updateTaskDto.Description;

        if (updateTaskDto.DueDate.HasValue)
            existingTask.DueDate = updateTaskDto.DueDate.Value;
        
        if (updateTaskDto.Status.HasValue)
            existingTask.TaskStatus = updateTaskDto.Status.Value;
        
        if (updateTaskDto.Priority.HasValue)
            existingTask.TaskPriority = updateTaskDto.Priority.Value;
    
        var updatedTask = await _taskRepository.UpdateTaskAsync(existingTask);
    
        _logger.LogInformation("Task {TaskId} updated successfully.", updatedTask.Id);
        return MapToResponseDto(updatedTask);
    }


    public async Task<bool> DeleteTaskAsync(Guid id, Guid userId)
    {
        _logger.LogInformation("Attempting to delete task {TaskId} for user {UserId}.", id, userId);

        if (!await _taskRepository.TaskExistsAsync(id, userId))
        {
            _logger.LogWarning("Task {TaskId} not found or user {UserId} lacks permission to delete it. No action taken.", id, userId);
            return false;
        }
        
        await _taskRepository.DeleteTaskAsync(id, userId);

        _logger.LogInformation("Task {TaskId} deleted successfully.", id);
        return true;
    }
    
    // A private helper method to map a database entity to a DTO for the API response.
    // This decouples the internal data model from the external API contract.
    private static TaskResponseDto MapToResponseDto(Entities.Task task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.TaskStatus,
            Priority = task.TaskPriority,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            UserId = task.UserId
        };
    }
}
