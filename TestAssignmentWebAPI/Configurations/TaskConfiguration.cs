using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = TestAssignmentWebAPI.Entities.Task;

namespace TestAssignmentWebAPI.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Entities.Task>
{
    public void Configure(EntityTypeBuilder<Entities.Task> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .IsRequired();
        
        builder.Property(t => t.Title)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(t => t.Description)
            .HasMaxLength(500);
        
        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(t => t.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(t => t.TaskStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.TaskPriority)
            .HasConversion<int>()
            .IsRequired();
        
        builder.HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}