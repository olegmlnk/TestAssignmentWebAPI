using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts.TaskDtos;

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
            throw new UnauthorizedAccessException("User does not own this task.");
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto taskDto, Guid userId)
    {
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

        return MapToResponseDto(createdTask);
    }

    public async Task<TaskResponseDto> GetTaskByIdAsync(Guid id, Guid userId)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id, userId);
        return task != null ? MapToResponseDto(task) : null;
    }

    public async Task<PaginationResultDto<TaskResponseDto>> GetAllTasksAsync(Guid userId, FilterTaskDto filter)
    {
        var result = await _taskRepository.GetTasksAsync(userId, filter);

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
        var existingTask = await _taskRepository.GetTaskByIdAsync(id, userId);

        if (existingTask == null)
            return null;

        if (!string.IsNullOrEmpty(updateTaskDto.Title))
            existingTask.Title = updateTaskDto.Title;
        
        if (updateTaskDto.Description != null)
            existingTask.Description = updateTaskDto.Description;
        
        if (updateTaskDto.DueDate.HasValue)
            existingTask.DueDate = updateTaskDto.DueDate.Value;
            
        if (updateTaskDto.Status.HasValue)
            existingTask.TaskStatus = updateTaskDto.Status.Value;
            
        if (updateTaskDto.Priority.HasValue)
            existingTask.TaskPriority = updateTaskDto.Priority.Value;
        
        
        var updatedTask = await _taskRepository.UpdateTaskAsync(existingTask);
        
        return MapToResponseDto(updatedTask);
    }

    public async Task<bool> DeleteTaskAsync(Guid id, Guid userId)
    {
        if (!await _taskRepository.TaskExistsAsync(id, userId))
            return false;
        
        await _taskRepository.DeleteTaskAsync(id, userId);

        return true;
    }
    
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