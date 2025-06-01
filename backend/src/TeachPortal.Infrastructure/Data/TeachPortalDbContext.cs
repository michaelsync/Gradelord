using Microsoft.EntityFrameworkCore;
using TeachPortal.Domain.Entities;

namespace TeachPortal.Infrastructure.Data;

public class TeachPortalDbContext : DbContext
{
    public TeachPortalDbContext(DbContextOptions<TeachPortalDbContext> options) : base(options)
    {
    }

    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Teacher entity
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.PasswordHash)
                .IsRequired();
                
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100);
                
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            // Configure relationship with Students
            entity.HasMany(e => e.Students)
                .WithOne(e => e.Teacher)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Student entity
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100);
                
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
} 