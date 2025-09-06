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

namespace TestAssignmentWebAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

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

            // Check if user already exists
            if (await _userRepository.GetByUsernameOrEmailAsync(registrationDto.Username) != null ||
                await _userRepository.GetByUsernameOrEmailAsync(registrationDto.Email) != null)
            {
                throw new InvalidOperationException("Username or email already exists");
            }

            // Create new user
            var user = new User
            {
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password)
            };

            var createdUser = await _userRepository.AddUserAsync(user);
            _logger.LogInformation("User registered successfully with ID: {UserId}", createdUser.Id);

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

        private string GenerateJwtToken(Guid userId, string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
}