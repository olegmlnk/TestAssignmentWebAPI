using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts;
using TestAssignmentWebAPI.Contracts.UserDtos;
using TestAssignmentWebAPI.Entities;
using TestAssignmentWebAPI.Exceptions;
using Task = System.Threading.Tasks.Task;
using Microsoft.Extensions.Configuration; // Required for accessing configuration settings
using Microsoft.Extensions.Logging; // Required for logging

namespace TestAssignmentWebAPI.Services;

// The UserService handles all business logic related to user management,
// including registration, login, and JWT token creation.
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    // The constructor uses dependency injection to get necessary services and configuration.
    public UserService(
        IUserRepository userRepository, 
        ILogger<UserService> logger, 
        IConfiguration configuration,
        AppDbContext context)
    {
        _userRepository = userRepository;
        _logger = logger;
        _configuration = configuration;
        _context = context;
    }

   
    public async Task<LoginResponseDto> RegisterAsync(RegisterDto registrationDto)
    {
        _logger.LogInformation("Attempting to register user with username: {Username}", registrationDto.Username);

        // Check if user already exists to prevent duplicate entries and provide a clear error message.
        if (await _userRepository.GetByUsernameOrEmailAsync(registrationDto.Username) != null ||
            await _userRepository.GetByUsernameOrEmailAsync(registrationDto.Email) != null)
        {
            _logger.LogWarning("Registration failed: Username or email already exists for {Username}", registrationDto.Username);
            throw new InvalidOperationException("Username or email already exists");
        }

        ValidatePassword(registrationDto.Password);
        // Create new user entity. Passwords are never stored in plain text;
        // they are hashed using BCrypt for security.
        var user = new User
        {
            Username = registrationDto.Username,
            Email = registrationDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password)
        };

        var createdUser = await _userRepository.AddUserAsync(user);
        _logger.LogInformation("User registered successfully with ID: {UserId}", createdUser.Id);

        // After successful registration, a JWT token is generated to allow the user
        // to immediately log in without a separate login call.
        var token = GenerateJwtToken(createdUser.Id, createdUser.Username);

        return new LoginResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                CreatedAt = createdUser.CreatedAt
            }
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation("Attempting to login user: {UsernameOrEmail}", loginDto.UsernameOrEmail);

        var user = await _userRepository.GetByUsernameOrEmailAsync(loginDto.UsernameOrEmail);
        
        // Verify the user exists and the provided password matches the stored hash.
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Invalid login attempt for: {UsernameOrEmail}", loginDto.UsernameOrEmail);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

        var token = GenerateJwtToken(user.Id, user.Username);

        return new LoginResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            }
        };
    }

    // This method handles the creation of a JWT token based on user information.
    private string GenerateJwtToken(Guid userId, string username)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"];
    
        if (string.IsNullOrEmpty(secret) || secret.Length < 32)
        {
            _logger.LogError("JWT Secret is not configured properly or is too short. It must be at least 32 characters.");
            throw new InvalidOperationException("JWT Secret is not configured properly");
        }

        // The secret key is converted to a byte array for cryptographic operations.
        var key = Encoding.ASCII.GetBytes(secret);

        // Claims are key-value pairs that contain information about the user.
        // These are stored inside the token and can be read by the client and the server.
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // Standard claim for user ID
            new Claim("nameid", userId.ToString()), // Alternative claim for user ID
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // JWT standard "subject" claim for user ID
            new Claim(ClaimTypes.Name, username), // Standard claim for username
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // "JWT ID" for token uniqueness
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64) // "Issued At" timestamp
        };

        // This object holds all the metadata needed to create the token.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Converts the token object into its final string representation.
        return tokenHandler.WriteToken(token);
    }
    
    private void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long");

        if (!password.Any(char.IsUpper))
            throw new ArgumentException("Password must contain at least one uppercase letter");

        if (!password.Any(char.IsLower))
            throw new ArgumentException("Password must contain at least one lowercase letter");

        if (!password.Any(char.IsDigit))
            throw new ArgumentException("Password must contain at least one digit");

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            throw new ArgumentException("Password must contain at least one special character");
    }

}
