using Microsoft.EntityFrameworkCore;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Entities;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Repositories;

// The UserRepository handles all data access logic for the User entity.
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    // The constructor uses dependency injection to get the database context and logger.
    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetUserByIdAsync(Guid? id)
    {
        _logger.LogInformation("Attempting to retrieve user with ID {UserId}.", id);
        
        if (id is null)
        {
            _logger.LogWarning("Attempted to retrieve user with a null ID.");
            return null;
        }

        var user = await _context.Users.FindAsync(id);      
        
        if (user == null)
            _logger.LogWarning("User with ID {UserId} not found.", id);
        else
            _logger.LogInformation("User with ID {UserId} found.", id);

        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        _logger.LogInformation("Retrieving all users from the database.");
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        _logger.LogInformation("Attempting to find user by username or email: {UsernameOrEmail}.", usernameOrEmail);
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == usernameOrEmail.ToLower() || 
                                      u.Email.ToLower() == usernameOrEmail.ToLower());
    }

    public async Task<User> AddUserAsync(User user)
    {
        _logger.LogInformation("Adding new user with username '{Username}'.", user.Username);
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation("User {UserId} successfully added.", user.Id);
        return user;
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        _logger.LogInformation("Updating user with ID {UserId}.", user.Id);
        user.UpdatedAt = DateTime.UtcNow;
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation("User {UserId} successfully updated.", user.Id);
        return user;
    }   

    public async Task DeleteUserAsync(Guid id)
    {
        _logger.LogInformation("Attempting to delete user with ID {UserId}.", id);
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User {UserId} successfully deleted.", id);
        }
        else
            _logger.LogWarning("Delete failed: User with ID {UserId} not found.", id);
    }
}
