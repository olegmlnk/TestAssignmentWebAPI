using System.Security.Claims;
using TestAssignmentWebAPI.Contracts;
using TestAssignmentWebAPI.Entities;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Abstractions;

public interface IUserService
{
    Task RegisterAsync(RegisterDto registerDto);
    Task<string> LoginAsync(LoginDto loginDto);
}