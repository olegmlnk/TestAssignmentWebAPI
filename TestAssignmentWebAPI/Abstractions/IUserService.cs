using System.Security.Claims;
using TestAssignmentWebAPI.Contracts;
using TestAssignmentWebAPI.Entities;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Abstractions;

public interface IUserService
{
    Task<RegisterDto> RegisterAsync(RegisterDto registerDto);
    Task<string> LoginAsync(LoginDto loginDto);
}