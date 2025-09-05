using TestAssignmentWebAPI.Entities;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Abstractions;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid? id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(Guid id);
}