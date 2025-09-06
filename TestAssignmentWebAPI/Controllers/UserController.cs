using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts;
using TestAssignmentWebAPI.Contracts.UserDtos;

namespace TestAssignmentWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public UserController(
        IUserService userService,
        ILogger<UserController> logger,
        IUserRepository userRepository)
    {
        _userService = userService;
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterDto registrationDto)
    {
        try
        {
            _logger.LogInformation("User registration attempt for username: {Username}", registrationDto.Username);

            var result = await _userService.RegisterAsync(registrationDto);

            _logger.LogInformation("User registration successful for username: {Username}", registrationDto.Username);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed for username {Username}: {Error}", registrationDto.Username,
                ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for username: {Username}",
                registrationDto.Username);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// Authenticate user and return JWT token
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
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
    
    [HttpGet("profile")]
    public async Task<ActionResult<UserResponseDto>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile for user: {UserId}", GetCurrentUserId());
            return StatusCode(500, new { message = "An error occurred while retrieving profile" });
        }
    }

    private Guid GetCurrentUserId()
    {
        _logger.LogInformation("Attempting to get current user ID from claims");
    
        // Використовуйте ClaimTypes.NameIdentifier замість JwtRegisteredClaimNames.Sub
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
        _logger.LogInformation("User ID claim value: {UserIdClaim}", userIdClaim);
    
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("No NameIdentifier claim found in token");
            throw new UnauthorizedAccessException("No user ID found in token");
        }
    
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid user ID format in token: {UserIdClaim}", userIdClaim);
            throw new UnauthorizedAccessException("Invalid user ID format in token");
        }
    
        _logger.LogInformation("Successfully parsed user ID: {UserId}", userId);
        return Guid.Parse(userIdClaim);
    }
}