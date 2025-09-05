using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestAssignmentWebAPI.Entities;

namespace TestAssignmentWebAPI.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id); // Auto-generated primary key
        
        builder.HasIndex(u => u.Username) // Unique index on Username
            .IsUnique();
        
        builder.HasIndex(u => u.Email) // Unique index on Email
            .IsUnique();
        
        builder.Property(u => u.Id)
            .IsRequired();

        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(u => u.Email)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP"); // also we can use NOW()
        
        builder.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Example of configuration when user has many tasksS
        builder.HasMany(u => u.Tasks)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}