using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TestAssignmentWebAPI.Configurations;
using TestAssignmentWebAPI.Entities;
using Task = TestAssignmentWebAPI.Entities.Task;
using Microsoft.Extensions.Configuration; // Required to use ConfigurationBuilder

namespace TestAssignmentWebAPI;

// The AppDbContext class represents the session with the database.
// It inherits from DbContext and is the main gateway for Entity Framework Core
// to perform database operations, such as querying and saving data.
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

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Manually build a configuration to read the connection string from appsettings.json.
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        var conn = config.GetConnectionString("TestAssignmentDbConnectionString");
        
        // Configure the DbContext to use PostgreSQL with the retrieved connection string.
        optionsBuilder.UseNpgsql(conn);
        
        return new AppDbContext(optionsBuilder.Options);
    }
}
