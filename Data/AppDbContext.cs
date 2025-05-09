using Microsoft.EntityFrameworkCore;
using Ai_LibraryApi.Models;

public class Ai_LibraryApiDbContext : DbContext
{
    public Ai_LibraryApiDbContext(DbContextOptions<Ai_LibraryApiDbContext> opts) : base(opts) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();
    public DbSet<Profile> Profiles => Set<Profile>();

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