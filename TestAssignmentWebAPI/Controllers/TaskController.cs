using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts;
using TestAssignmentWebAPI.Contracts.TaskDtos;
using TestAssignmentWebAPI.Entities;

namespace TestAssignmentWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;
    
    public TaskController(ITaskService taskService, ILogger<TaskController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateTaskAsync([FromBody] CreateTaskDto createTaskDto)
    {
        try
        {
            // Retrieves the user's Id from the jwt token. This is a
            // step to ensure the task is associated with the correct user.
            var userId = GetCurrentUserId();
                
            _logger.LogInformation("Creating new task for user {UserId}", userId);
                
            var createdTask = await _taskService.CreateTaskAsync(createTaskDto, userId);
                
            _logger.LogInformation("Task {TaskId} created successfully for user {UserId}", createdTask.Id, userId);
                
            return Ok(createdTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task for user: {UserId}", GetCurrentUserId());
            
            return StatusCode(500, new { message = "An error occurred while creating the task" });
        }
    }
    
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateTaskAsync(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Attempting to update task {TaskId} for user {UserId}", id, userId);

            var updatedTask = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);
            
            // If the task is not found or the user doesn't have permission,
            // the service returns null, which is handled here by returning a 404 Not Found.
            if(updatedTask == null)
            {
                _logger.LogWarning("Task {TaskId} not found or user {UserId} lacks permission to update it.", id, userId);
                return NotFound(new { message = "Task not found or you do not have permission to update it." });
            }

            _logger.LogInformation("Task {TaskId} updated successfully.", updatedTask.Id);
            return Ok(updatedTask);
        }
        catch (Exception e)
        {
           _logger.LogError(e, "Error updating task {TaskId} for user: {UserId}", id, GetCurrentUserId());
           return StatusCode(500, new { message = "An error occurred while updating the task" });
        }
    }
    
    [HttpGet("getAll")]
    public async Task<ActionResult<PaginationResultDto<TaskResponseDto>>> GetUserTasksAsync(
        [FromQuery] Status? status = null,
        [FromQuery] Priority? priority = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] string? sortOrder = "desc",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Retrieving tasks for user {UserId} with filters: Status={Status}, Priority={Priority}, Page={PageNumber}, PageSize={PageSize}", userId, status, priority, pageNumber, pageSize);
            
            // Clamps page number and size to valid ranges to prevent invalid queries.
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var filter = new FilterTaskDto
            {
                Status = status,
                Priority = priority,
                DueDateFrom = dueDateFrom,
                DueDateTo = dueDateTo,
                SortBy = sortBy,
                SortOrder = sortOrder,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _taskService.GetAllTasksAsync(userId, filter);

            _logger.LogInformation("Successfully retrieved {Count} tasks for user {UserId}.", result.TotalCount, userId);
            return Ok(result);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for user: {UserId}", GetCurrentUserId());
            return StatusCode(500, new { message = "An error occurred while retrieving tasks" });
        }
    }
    
    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetTaskByIdAsync(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Attempting to retrieve task {TaskId} for user {UserId}.", id, userId);

            var task = await _taskService.GetTaskByIdAsync(id, userId);

            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found for user {UserId}.", id, userId);
                return NotFound(new { message = "Task not found or you do not have access to it." });
            }

            _logger.LogInformation("Successfully retrieved task {TaskId}.", id);
            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId} for user: {UserId}", id, GetCurrentUserId());
            return StatusCode(500, new { message = "An error occurred while retrieving the task" });
        }
    }
    
    [NonAction]
    private Guid GetCurrentUserId()
    {
        // Finds the user ID claim. The NameIdentifier is the standard claim type for the user ID.
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        // Ensures the claim exists and its value is a valid GUID.
        if(string.IsNullOrEmpty(userIdClaim?.Value) || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID.");
        }

        return userId;
    }
}
