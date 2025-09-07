namespace TestAssignmentWebAPI.Exceptions;

public class UserRegistrationException(string message) : Exception($"User registration failed: {message}");