using System.Security.Claims;
using TestAssignmentWebAPI.Contracts;
using TestAssignmentWebAPI.Contracts.UserDtos;
using TestAssignmentWebAPI.Entities;
using Task = System.Threading.Tasks.Task;

namespace TestAssignmentWebAPI.Abstractions;

public interface IUserService
{
    Task<LoginResponseDto> RegisterAsync(RegisterDto registrationDto);
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
}