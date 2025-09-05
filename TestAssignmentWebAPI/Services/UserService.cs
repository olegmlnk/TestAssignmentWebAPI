using System.Security.Claims;
using TestAssignmentWebAPI.Abstractions;
using TestAssignmentWebAPI.Entities;

namespace TestAssignmentWebAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<User> RegisterAsync()
    { 
        throw new NotImplementedException();
    }

    public Task<User> LoginAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUserAsync(User user)
    {   
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user, List<Claim> roleClaims, IList<Claim> userClaims)
    {
        throw new NotImplementedException();
    }
}