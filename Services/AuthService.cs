
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
 
using UserSystem.API.DTOs;
using UserSystem.Models;

namespace UserSystem.API.Services;


public interface IAuthService
{
    Task<AuthResponseDto> RegisterUserAsync(UserRegisterDto dto);
    Task<AuthResponseDto> RegisterAdminAsync(AdminRegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    //Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
}


public class AuthService : IAuthService
{
    private readonly UserSystemDbContext _dbcontext;
    private readonly IConfiguration _cfg;
    private readonly PasswordHasher<object> _hasher = new();

    public AuthService(UserSystemDbContext ctx, IConfiguration cfg)
    {
        _dbcontext = ctx;
        _cfg = cfg;
    }


 /*   public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _dbcontext.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.Name,
            Email = dto.Email,
            PasswordHash = _hasher.HashPassword(null!, dto.Password)
        };
        var admin = new Admin
        {
            Id = Guid.NewGuid(),

            AdminName = dto.Name,
            Email = dto.Email,
            PasswordHash = _hasher.HashPassword(null!, dto.Password)
        };
        _dbcontext.Admins.Add(admin);
        _dbcontext.Users.Add(user);
        await _dbcontext.SaveChangesAsync();

        _dbcontext.RoleAssignments.Add(new RoleAssignment { UserId = user.Id, RoleId = 1 });
        await _dbcontext.SaveChangesAsync();

        return GenerateJwt(user.Email, "User");
    }*/

    public async Task<AuthResponseDto> RegisterUserAsync(UserRegisterDto dto)
    {
        if (await _dbcontext.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            Email = dto.Email,
            PasswordHash = _hasher.HashPassword(null!, dto.Password)
        };

        _dbcontext.Users.Add(user);
        await _dbcontext.SaveChangesAsync();

        _dbcontext.RoleAssignments.Add(new RoleAssignment { UserId = user.Id, RoleId = 1 });
        await _dbcontext.SaveChangesAsync();

        return GenerateJwt(user.Email, "User");
    }

    public async Task<AuthResponseDto> RegisterAdminAsync(AdminRegisterDto dto)
    {
        if (await _dbcontext.Admins.AnyAsync(a => a.Email == dto.Email))
            throw new InvalidOperationException("Email already in use.");

        var admin = new Admin
        {
            Id = Guid.NewGuid(),
            AdminName = dto.AdminName,
            Email = dto.Email,
            PasswordHash = _hasher.HashPassword(null!, dto.Password)
        };

        _dbcontext.Admins.Add(admin);
        await _dbcontext.SaveChangesAsync();

        _dbcontext.RoleAssignments.Add(new RoleAssignment { AdminId = admin.Id, RoleId = 2 });
        await _dbcontext.SaveChangesAsync();

        return GenerateJwt(admin.Email, "Admin");
    }
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _dbcontext.Users
            .Include(u => u.RoleAssignments)
            .ThenInclude(ra => ra.Role)
            .FirstOrDefaultAsync(u => u.UserName == dto.Name);

        if (user != null && _hasher.VerifyHashedPassword(null!, user.PasswordHash, dto.Password) == PasswordVerificationResult.Success)
        {
            var role = user.RoleAssignments.FirstOrDefault()?.Role?.Name;
            if (role == null) throw new UnauthorizedAccessException("Role not assigned.");
            return GenerateJwt(user.Email, role);
        }

        var admin = await _dbcontext.Admins
            .Include(a => a.RoleAssignments)
            .ThenInclude(ra => ra.Role)
            .FirstOrDefaultAsync(a => a.AdminName == dto.Name);

        if (admin != null && _hasher.VerifyHashedPassword(null!, admin.PasswordHash, dto.Password) == PasswordVerificationResult.Success)
        {
            var role = admin.RoleAssignments.FirstOrDefault()?.Role?.Name;
            if (role == null) throw new UnauthorizedAccessException("Role not assigned.");
            return GenerateJwt(admin.AdminName, role);

            
        }

        throw new UnauthorizedAccessException("Invalid credentials.");
    }

    
    // Helper
    private AuthResponseDto GenerateJwt(string name, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(ClaimTypes.Role, role)
            },
            expires: expires,
            signingCredentials: creds);

        return new AuthResponseDto(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
