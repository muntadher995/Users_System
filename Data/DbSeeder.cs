/*using Ai_LibraryApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Ai_LibraryApi.Data
{
    public class DbSeederLogger { }

    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Ai_LibraryApiDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DbSeeder");

            try
            {
                logger.LogInformation("Starting database migration");
                await context.Database.MigrateAsync();

                // Seed Roles
                if (!await context.Roles.AnyAsync())
                {
                    logger.LogInformation("Seeding roles");
                    await context.Roles.AddRangeAsync(
                        new Role { Id = 1, Name = "User" },
                        new Role { Id = 2, Name = "Admin" }
                    );
                    await context.SaveChangesAsync();
                }

                // Seed Admin
                if (!await context.Admins.AnyAsync())
                {
                    logger.LogInformation("Seeding admin user");
                    var pwHasher = new PasswordHasher<object>();
                    var admin = new Admin
                    {
                        Id = Guid.NewGuid(),
                        AdminName = "Admin",
                        Email = "admin@system.local",
                        PasswordHash = pwHasher.HashPassword(null!, "Admin@123"),
                        isActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await context.Admins.AddAsync(admin);

                    // Add role assignment for admin
                    await context.RoleAssignments.AddAsync(new RoleAssignment
                    {
                         AdminId = admin.Id,
                        RoleId = 2 // Admin role
                    });

                    await context.SaveChangesAsync();
                    logger.LogInformation($"Admin user created with ID: {admin.Id}");
                }

                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw; // Re-throw to ensure the error is visible
            }
        }
    }
}


















*//*using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ai_LibraryApi.Models;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<Ai_LibraryApiDbContext>();
        await ctx.Database.MigrateAsync();

        var pwHasher = new PasswordHasher<object>();

        if (!await ctx.Admins.AnyAsync())
        {
            var admin = new Admin
            {
                Id = Guid.NewGuid(),
                AdminName = "Admin",
                Email = "admin@system.local",
                PasswordHash = pwHasher.HashPassword(null!, "Admin@123"),
                isActive= true

            };
            ctx.Admins.Add(admin);

            ctx.RoleAssignments.Add(new RoleAssignment
            {
                Admin = admin,
                RoleId = 2 // “Admin”
            });
            await ctx.SaveChangesAsync();
        }
    }
}*/