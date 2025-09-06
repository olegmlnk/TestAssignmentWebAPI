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
    [Authorize]
    public async Task<IActionResult> CreateTaskAsync([FromBody] CreateTaskDto createTaskDto)
    {
        try
        {
            var userId = GetCurrentUserId();
                
            _logger.LogInformation("Creating new task for user {UserId}", userId);
                
            var createdTask = await _taskService.CreateTaskAsync(createTaskDto, userId);
                
            _logger.LogInformation("Task {TaskId} created successfully for user {UserId}", createdTask.Id, userId);
                
            return CreatedAtAction(nameof(GetTaskByIdAsync), new { id = createdTask.Id }, createdTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task for user: {UserId}", GetCurrentUserId());
            return StatusCode(500, new { message = "An error occurred while creating the task" });
        }
    }
    
    [HttpPut("update/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTaskAsync(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        try
        {
            var userId = GetCurrentUserId();

            var updatedTask = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);
            
            if(updatedTask == null)
                return NotFound(new { message = "Task not found or you do not have permission to update it." });

            return Ok(updatedTask);
        }
        catch (Exception e)
        {
           return StatusCode(500, new { message = "An error occurred while updating the task" });
        }
    }
    
    [HttpGet("getAll")]
    [Authorize]
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

            return Ok(result);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for user: {UserId}", GetCurrentUserId());
            return StatusCode(500, new { message = "An error occurred while retrieving tasks" });
        }
    }
    
    [HttpGet("getById/{id}")]
    [Authorize]
    public async Task<IActionResult> GetTaskByIdAsync(Guid id, Guid userId)
    {
        var task = await _taskService.GetTaskByIdAsync(id, userId);
        return Ok(task);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userIdClaim?.Value) || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID.");
        }

        return userId;
    }
}