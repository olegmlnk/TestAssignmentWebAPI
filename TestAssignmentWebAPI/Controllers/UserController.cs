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
            // This block handles a specific case, for example, when a user
            // with the same name already exists. This allows returning a
            // specific 400 Bad Request error.
            _logger.LogWarning("Registration failed for username {Username}: {Error}", registrationDto.Username,
                ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // This block handles any other unexpected errors, such as
            // database issues, and returns a generic 500 Internal Server
            // Error to avoid exposing implementation details to the client.
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
            // This exception handles authorization errors like an incorrect
            // password or a non-existent user. It returns a 401 Unauthorized.
            _logger.LogWarning("Login failed for {UsernameOrEmail}: {Error}", loginDto.UsernameOrEmail, ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // A generic block for unexpected errors to return a 500
            // Internal Server Error.
            _logger.LogError(ex, "Unexpected error during login for: {UsernameOrEmail}", loginDto.UsernameOrEmail);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }
    
    [HttpGet("profile")]
    [Authorize] // This attribute requires a valid JWT token to be provided
                // by the user to access the method.
    public async Task<ActionResult<UserResponseDto>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                // Returns a 404 Not Found if the user is not found,
                // even if the token is valid.
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            // Handles any errors that occurred while retrieving the profile
            // (e.g., database issues).
            _logger.LogError(ex, "Error retrieving user profile for user: {UserId}", GetCurrentUserId());
            return StatusCode(500, new { message = "An error occurred while retrieving profile" });
        }
    }

    // A private method for securely getting the user ID from the token.
    [NonAction] // This attribute prevents direct access to the method via
                // an HTTP request, as it is an internal controller function.
    private Guid GetCurrentUserId()
    {
        _logger.LogInformation("Attempting to get current user ID from claims");

        // Using LINQ to find the claim that contains the user ID. This
        // improves reliability as different providers might use different
        // claim names ("sub", "nameid", etc.).
        var userIdClaimValue = User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid" || c.Type == "sub" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
            .Select(c => c.Value)
            .FirstOrDefault();

        _logger.LogInformation("User ID claim value found: {UserIdClaimValue}", userIdClaimValue ?? "null");

        if (string.IsNullOrEmpty(userIdClaimValue))
        {
            _logger.LogWarning("No valid user ID claim found in token.");
            throw new UnauthorizedAccessException("No user ID found in token.");
        }

        // Tries to parse the string claim value into a Guid object.
        // If the string has an invalid format, the method returns false,
        // which allows us to handle the error correctly.
        if (!Guid.TryParse(userIdClaimValue, out var userId))
        {
            _logger.LogWarning("Invalid user ID format in token: {UserIdClaimValue}", userIdClaimValue);
            throw new UnauthorizedAccessException("Invalid user ID format in token.");
        }

        _logger.LogInformation("Successfully parsed user ID: {UserId}", userId);
        return userId;
    }
}
