using Microsoft.AspNetCore.Mvc;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts;

namespace TestAssignmentWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserService userService,
        ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
    {
        await _userService.RegisterAsync(registerDto);
        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
    {
        await _userService.LoginAsync(loginDto);
        return Ok(new { Message = "User logged in successfully." });
    }
}