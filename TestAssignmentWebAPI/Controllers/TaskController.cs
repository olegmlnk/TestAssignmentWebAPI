using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts;
using TestAssignmentWebAPI.Contracts.TaskDtos;

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
        //await _taskService.CreateTaskAsync();
        return Ok(new { Message = "Task created successfully." });
    }
    
    [HttpPut("update/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTaskAsync(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
    {
       // await _taskService.UpdateTaskAsync(id, updateTaskDto);
        return Ok(new { Message = "Task updated successfully." });
    }
    
    [HttpGet("getById/{id}")]
    [Authorize]
    public async Task<IActionResult> GetTaskByIdAsync(Guid id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        return Ok(task);
    }
}