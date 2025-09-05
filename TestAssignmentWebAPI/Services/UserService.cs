using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Contracts;
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

    public async Task RegisterAsync(RegisterDto registerDto)
    { 
        var userExists = await _userRepository.GetUserByEmailAsync(registerDto.Email);

        if (userExists != null)
        {
            throw new UserRegistrationException("User with this email already exists.");
        }
        
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        var result = await _userRepository.AddUserAsync(user);
        if (result == null) 
        {
            throw new UserRegistrationException("User registration failed. Please try again.");
        }
    }

    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Email == loginDto.UsernameOrEmail || u.Username == loginDto.UsernameOrEmail);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UserLoginException("Invalid username/email or password.");
        }

        return GenerateJwtToken(user);

    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtOptions");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationTimeInMinutes"])),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}