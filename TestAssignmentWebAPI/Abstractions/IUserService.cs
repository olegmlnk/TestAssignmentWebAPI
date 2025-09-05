using System.Security.Claims;
using TestAssignmentWebAPI.Entities;

namespace TestAssignmentWebAPI.Abstractions;

public interface IUserService
{
    Task<User> RegisterAsync();
    Task<User> LoginAsync();
    Task<User> GetUserByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid id);
    (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(
        User user, 
        List<Claim> roleClaims,
        IList<Claim> userClaims);
}