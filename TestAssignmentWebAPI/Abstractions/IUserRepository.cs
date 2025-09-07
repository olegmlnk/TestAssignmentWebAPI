using TestAssignmentWebAPI.Entities;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Abstractions;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid? id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    Task<User> AddUserAsync(User user);
    Task<User?> UpdateUserAsync(User user);
    Task DeleteUserAsync(Guid id);
}