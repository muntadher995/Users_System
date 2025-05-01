using Microsoft.EntityFrameworkCore;
using UserSystem.Models;

public class UserSystemDbContext : DbContext
{
    public UserSystemDbContext(DbContextOptions<UserSystemDbContext> opts) : base(opts) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Role>().HasData(
            new Role { Id = 1, Name = "User" },
            new Role { Id = 2, Name = "Admin" });

        // Unique indexes
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();
        b.Entity<Admin>().HasIndex(a => a.Email).IsUnique();
    }
}