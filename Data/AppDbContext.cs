using Microsoft.EntityFrameworkCore;
using Ai_LibraryApi.Models;
using Microsoft.AspNetCore.Identity;

public class Ai_LibraryApiDbContext : DbContext
{
    public Ai_LibraryApiDbContext(DbContextOptions<Ai_LibraryApiDbContext> opts) : base(opts) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Degree> Degrees => Set<Degree>();
     
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Category> Categories => Set<Category>();
     

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

         
        // إضافة الأدوار
        b.Entity<Role>().HasData(
            new Role { Id = 1, Name = "User" },
            new Role { Id = 2, Name = "Admin" });

        // إضافة مسؤول افتراضي
        var adminId = Guid.NewGuid();
        var passwordHasher = new PasswordHasher<Admin>();
        var defaultAdmin = new Admin
        {
            Id = adminId,
            AdminName = "admin",
            Email = "admin@example.com",
            PasswordHash = passwordHasher.HashPassword(null, "Admin123!"),
            isActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        b.Entity<Admin>().HasData(defaultAdmin);

        // إضافة تعيين دور المسؤول
        b.Entity<RoleAssignment>().HasData(
            new RoleAssignment
            {
                Id = 1,
                RoleId = 2, // دور المسؤول
                AdminId = adminId
            });

        // Define the relationship between Profile and Degree
        b.Entity<Degree>()
            .HasOne(d => d.Profile)
            .WithMany(p => p.Degrees)
            .HasForeignKey(d => d.ProfileId)
            .OnDelete(DeleteBehavior.Cascade); // Optional: Specify delete behavior
    
    b.Entity<Profile>()
            .HasOne(p => p.User)
            .WithMany(u => u.Profiles)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Notification>()
          .HasOne(n => n.User)
          .WithMany(u => u.Notifications)
          .HasForeignKey(n => n.UserId)
          .OnDelete(DeleteBehavior.Cascade);

    }
}