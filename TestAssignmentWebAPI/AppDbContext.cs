using Microsoft.EntityFrameworkCore;

namespace TestAssignmentWebAPI;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}