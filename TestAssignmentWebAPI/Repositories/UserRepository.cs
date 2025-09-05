using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Entities;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<User> AddUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}