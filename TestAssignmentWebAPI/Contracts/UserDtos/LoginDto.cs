namespace TestAssignmentWebAPI.Contracts;

public record LoginDto 
(
    string UsernameOrEmail, 
    string Password
);