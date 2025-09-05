using Microsoft.EntityFrameworkCore;
using TestAssignmentWebAPI.Configurations;
using TestAssignmentWebAPI.Entities;
using Task = TestAssignmentWebAPI.Entities.Task;

namespace TestAssignmentWebAPI;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
    }
}