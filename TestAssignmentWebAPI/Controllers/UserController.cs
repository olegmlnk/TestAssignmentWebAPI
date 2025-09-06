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
        try
        {
            _logger.LogInformation("User registration attempt for username: {Username}", registerDto.Username);
                
            var result = await _userService.RegisterAsync(registerDto);
                
            _logger.LogInformation("User registration successful for username: {Username}", registerDto.Username);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed for username {Username}: {Error}", registerDto.Username, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for username: {Username}", registerDto.Username);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("User login attempt for: {UsernameOrEmail}", loginDto.UsernameOrEmail);
                
            var result = await _userService.LoginAsync(loginDto);
                
            _logger.LogInformation("User login successful for: {UsernameOrEmail}", loginDto.UsernameOrEmail);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed for {UsernameOrEmail}: {Error}", loginDto.UsernameOrEmail, ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for: {UsernameOrEmail}", loginDto.UsernameOrEmail);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }
}