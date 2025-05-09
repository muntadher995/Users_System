/*using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserSystem.Models;

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
                AdminName = "SuperAdmin",
                Email = "admin@system.local",
                PasswordHash = pwHasher.HashPassword(null!, "Admin@123")
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