# TestAsignmentWebAPI
ASP.NET 8.0 Web API for task management with user authentication, built using clean architecture principles and modern development practices.

## Features

### User Management
- Secure Registration: User registration with BCrypt password hashing
- JWT Authentication: Stateless authentication using JSON Web Tokens
- Password Security: Complex password requirements with validation
- Unique Constraints: Enforced unique usernames and email addresses
- User Profiles: Get current user profile information

### Task Management
- CRUD Operations: Create, Read, Update, Delete tasks
- Filtering: Filter by status, priority, and due date ranges
- Sorting: Sort by creation date, due date, priority, or title
- Pagination: Efficient pagination for large task lists
- User Isolation: Users can only access their own tasks

### Security & Architecture
- JWT-based Authorization: Secure endpoint protection
- Repository Pattern: Clean separation of data access logic
- Service Layer: Business logic abstraction
- Dependency Injection: Loose coupling and testability
- Logging: Detailed operation tracking
- Input Validation: Data annotations and custom validation

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or Full Edition)
- Visual Studio 2022 or Visual Studio Code
- Git for version control

## Installation & Setup

### 1. Clone the Repository
git clone https://github.com/olegmlnk/TestAssignmentWebAPI.git
cd TestAssignmentWebAPI

### 2. Install Dependencies
dotnet restore

### 3. Configure Database Connection
Update appsettings.json with your SQL Server connection:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagementDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}

Alternative connection strings:
- SQL Server Express: "Server=.\\SQLEXPRESS;Database=TaskManagementDB;Trusted_Connection=true;TrustServerCertificate=true;"
- LocalDB: "Server=(localdb)\\MSSQLLocalDB;Database=TaskManagementDB;Trusted_Connection=true;"
- Docker SQL Server: "Server=localhost,1433;Database=TaskManagementDB;User Id=SA;Password=YourPassword123;TrustServerCertificate=true;"

### 4. Configure JWT Settings
Ensure secure JWT configuration in appsettings.json:

{
  "JwtSettings": {
    "Secret": "ThisIsAVerySecureSecretKeyThatShouldBeAtLeast32CharactersLong!",
    "ExpiryInMinutes": 15
  }
}

Security Note: Use a different, secure secret in production (minimum 32 characters).

### 5. Database Setup
The database will be created automatically on first run. Alternatively, use Entity Framework migrations:

# Create migration (if needed)
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

### 6. Run the Application
dotnet run

Application URLs:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: https://localhost:5001/swagger

## API Documentation

### Authentication Flow

#### 1. Register New User
POST /api/User/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}

Password Requirements:
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character

Success Response (201 Created):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "john_doe",
    "email": "john.doe@example.com",
    "createdAt": "2025-09-06T15:30:00Z"
  }
}

#### 2. User Login
POST /api/User/login
Content-Type: application/json

{
  "usernameOrEmail": "john_doe",
  "password": "SecurePass123!"
}

Success Response (200 OK):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "john_doe",
    "email": "john.doe@example.com",
    "createdAt": "2025-09-06T15:30:00Z"
  }
}

#### 3. Get User Profile
GET /api/User/profile
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

### Task Management

Note: All task endpoints require JWT authentication via Authorization: Bearer <token> header.

#### 1. Create Task
POST /api/Task/create
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "title": "Complete project documentation",
  "description": "Write comprehensive API documentation with examples",
  "dueDate": "2025-12-31T23:59:59Z",
  "status": 0,
  "priority": 2
}

Enum Values:
- Status: 1 = Pending, 2 = InProgress, 3 = Completed
- Priority: 1 = Low, 2 = Medium, 3 = High

Success Response (201 Created):
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "title": "Complete project documentation",
  "description": "Write comprehensive API documentation with examples",
  "dueDate": "2025-12-31T23:59:59Z",
  "status": 1,
  "priority": 2,
  "createdAt": "2025-09-06T15:45:00Z",
  "updatedAt": "2025-09-06T15:45:00Z",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}

#### 2. Get Tasks with Filtering & Pagination
GET /api/Task/getAll?status=0&priority=2&dueDateFrom=2025-09-01&dueDateTo=2025-12-31&sortBy=DueDate&sortOrder=asc&page=1&pageSize=10
Authorization: Bearer <jwt-token>

Query Parameters:
| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| status | int? | Filter by status (1=Pending, 2=InProgress, 3=Completed) | null |
| priority | int? | Filter by priority (1=Low, 2=Medium, 3=High) | null |
| dueDateFrom | DateTime? | Filter tasks from this date | null |
| dueDateTo | DateTime? | Filter tasks until this date | null |
| sortBy | string | Sort field (CreatedAt, DueDate, Priority, Title) | "CreatedAt" |
| sortOrder | string | Sort direction (asc, desc) | "desc" |
| page | int | Page number | 1 |
| pageSize | int | Items per page (max 100) | 10 |

Success Response (200 OK):
{
  "items": [
    {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "title": "Complete project documentation",
      "description": "Write comprehensive API documentation",
      "dueDate": "2025-12-31T23:59:59Z",
      "status": 1,
      "priority": 2,
      "createdAt": "2025-09-06T15:45:00Z",
      "updatedAt": "2025-09-06T15:45:00Z",
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}

#### 3. Get Single Task
GET /api/Task/getById/{id}
Authorization: Bearer <jwt-token>

#### 4. Update Task
PUT /api/Task/update/{id}
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "title": "Updated task title",
  "description": "Updated description",
  "status": 1,
  "priority": 1
}

Note: All fields are optional in update requests. Only provided fields will be updated.

#### 5. Delete Task
DELETE /api/Task/delete/{id}
Authorization: Bearer <jwt-token>

Success Response: 204 No Content

## Architecture & Design
### Project Structure
TestAssignmentWebAPI/
├── Controllers/           # HTTP request handlers
      UsersController.cs
      TasksController.cs
├── Services/             # Business logic layer
      UserService.cs
      TaskService.cs
├── Abstractions          #Abstraction layer
      IUserRepository.cs
      ITaskRepository.cs
      IUserService.cs
      ITaskService.cs
├── Repositories/         # Data access layer
      UserRepository.cs
      TaskRepository.cs
├── Entities/              # Domain entities
      User.cs
      Task.cs
├── Contracts/  # Data transfer objects
     ├──TaskDtos/
         CreateTaskDto.cs
         FilterTaskDto.cs
         PaginationResultDto.cs
         TaskResponseDto.cs
         UpdateTaskDto.cs
     ├──UserDtos/
         LoginDto.cs
         RegisterDto.cs
         LoginResponseDto.cs
         UserResponseDto.cs
└── ApplicationDbContext.cs  #Database layer
└── Program.cs           # Application entry point

### Design Patterns

#### 1. Repository Pattern
- Purpose: Abstracts data access logic from business logic
- Benefits: Testability, maintainability, loose coupling
- Implementation: Generic interfaces with Entity Framework implementations

public interface ITaskRepository
{
    Task<Entities.Task?> GetTaskByIdAsync(Guid? id, Guid userId);
    Task<PaginationResultDto<Entities.Task>> GetTasksAsync(Guid userId, FilterTaskDto filterTaskDto);
    Task<IEnumerable<Entities.Task>> GetAllTasksAsync();
    Task<Entities.Task> AddTaskAsync(Entities.Task task);
    Task<Entities.Task> UpdateTaskAsync(Entities.Task? task);
    Task DeleteTaskAsync(Guid? id, Guid userId);
    Task<bool> TaskExistsAsync(Guid? id, Guid userId);
}

#### 2. Service Layer Pattern
- Purpose: Encapsulates business logic and rules
- Benefits: Single responsibility, reusability, testability
- Implementation: Services coordinate between controllers and repositories

public interface ITaskService
{
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto taskDto, Guid userId);
    Task<TaskResponseDto> GetTaskByIdAsync(Guid id, Guid userId);
    Task<PaginationResultDto<TaskResponseDto>> GetAllTasksAsync(Guid userId, FilterTaskDto filter);
    Task<TaskResponseDto?> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto, Guid userId);
    Task<bool> DeleteTaskAsync(Guid id, Guid userId);
}

#### 3. Dependency Injection
- Purpose: Promotes loose coupling and testability
- Benefits: Easy unit testing, configuration flexibility
- Implementation: Built-in ASP.NET Core DI container

// Program.cs
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();

#### 4. DTO Pattern
- Purpose: Defines data contracts for API communication
- Benefits: Versioning, security, validation
- Implementation: Separate DTOs for requests and responses

### Security Implementation

#### JWT Authentication
- Stateless: No server-side session storage
- Claims-based: User information embedded in token
- Secure: HMAC SHA256 signature verification

#### Password Security
- BCrypt Hashing: Industry-standard password hashing
- Salt Generation: Automatic salt generation per password
- Complexity Requirements: Enforced password strength

#### Authorization
- Role-based: Built on ASP.NET Core Identity claims
- Resource-based: Users can only access their own data
- Attribute-based: [Authorize] attribute protection

### 🗄 Database Design

#### User Table
## Users

Id (GUID, PK)

Username (Unique)

Email (Unique)

PasswordHash

CreatedAt / UpdatedAt

## Tasks

Id (GUID, PK)

Title

Description

DueDate

Status

Priority

UserId (FK Users)

Indexes:

Users: Username, Email

Tasks: UserId, composite (UserId, Status, Priority, DueDate)

#### Indexes
- Users: Unique indexes on Username and Email
- Tasks: Index on UserId for efficient user-scoped queries
- Tasks: Composite index on (UserId, Status, Priority, DueDate)

## Testing
### Using Swagger UI
1. Navigate to: https://localhost:5001/swagger
2. Register a new user via /api/users/register
3. Copy the returned JWT token
4. Click the "Authorize" button
5. Enter: Bearer <your-jwt-token>
6. Test all protected endpoints

### Using Postman
1. Create a new collection
2. Set collection-level authorization to "Bearer Token"
3. Add your JWT token to the collection
4. Import the API endpoints or create requests manually

## Configuration

### Environment-Specific Settings

#### Development (appsettings.Development.json)
{
  "ConnectionStrings": {
     "TestAssignmentDbConnectionString": "Host=localhost;Port=5432;Database=TestAssignmentDB;Username=postgres;Password=Guitareagle237"
  },
  "JwtSettings": {
    "Secret": "DevelopmentSecretKeyThatIsLongEnough32Chars!",
    "ExpiryInDays": 1
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "TaskManagement": "Debug"
    }
  }
}

#### Production
# Environment Variables
export ConnectionStrings__DefaultConnection="Server=prod-server;Database=TaskManagementDB;..."
export JwtSettings__Secret="YourProductionSecretKey32CharsMinimum!"
export JwtSettings__ExpiryInMinutes="15"

## Production Deployment

### Security Checklist
- [ ] Use strong JWT secrets (32+ characters)
- [ ] Enable HTTPS only
- [ ] Secure database connection strings
- [ ] Enable request/response logging
- [ ] Use environment-specific configurations
- [ ] Regular security updates

### Performance Considerations
- Database connection pooling
- Response caching for statistics
- Pagination for large datasets
- Database indexing optimization
- JWT token expiration management

## Contributing

1. Fork the repository
2. Follow the existing code patterns and conventions
3. Add comprehensive logging for new features
4. Include input validation and error handling
5. Update documentation for API changes
6. Commit changes (git commit -m 'Add AmazingFeature')
7. Push to branch (git push origin feature/AmazingFeature)
8. Open a Pull Request

Built using ASP.NET Core 8.0, Entity Framework Core, and modern software architecture principles.
