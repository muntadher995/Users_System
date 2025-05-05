
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
    Task<AuthResponseDto> LoginUserAsync(LoginDto dto);
    Task<AuthResponseDto> LoginAdminAsync(LoginDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
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

        return await CreateTokensAndUpdateRefreshToken(user.Email, "User", "User");
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

        return await CreateTokensAndUpdateRefreshToken(admin.Email, "Admin", "Admin");
    }
    public async Task<AuthResponseDto> LoginUserAsync(LoginDto dto)
    {
        var user = await _dbcontext.Users
            .Include(u => u.RoleAssignments)
            .ThenInclude(ra => ra.Role)
            .FirstOrDefaultAsync(u => u.UserName == dto.Name);

        if (user != null && _hasher.VerifyHashedPassword(null!, user.PasswordHash, dto.Password) == PasswordVerificationResult.Success)
        {
            var role = user.RoleAssignments.FirstOrDefault()?.Role?.Name;
            if (role == null) throw new UnauthorizedAccessException("Role not assigned.");
            if (role != "User") throw new UnauthorizedAccessException("Invalid user role.");
            return await CreateTokensAndUpdateRefreshToken(user.Email, role, "User");
        }

        throw new UnauthorizedAccessException("Invalid user credentials.");
    }

    public async Task<AuthResponseDto> LoginAdminAsync(LoginDto dto)
    {
        var admin = await _dbcontext.Admins
            .Include(a => a.RoleAssignments)
            .ThenInclude(ra => ra.Role)
            .FirstOrDefaultAsync(a => a.AdminName == dto.Name);

        if (admin != null && _hasher.VerifyHashedPassword(null!, admin.PasswordHash, dto.Password) == PasswordVerificationResult.Success)
        {
            var role = admin.RoleAssignments.FirstOrDefault()?.Role?.Name;
            if (role == null) throw new UnauthorizedAccessException("Role not assigned.");
            if (role != "Admin") throw new UnauthorizedAccessException("Invalid admin role.");
            return await CreateTokensAndUpdateRefreshToken(admin.Email, role, "Admin");
        }

        throw new UnauthorizedAccessException("Invalid admin credentials.");
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

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthResponseDto(accessToken, GenerateRefreshToken(), expires);
    }

    private async Task<AuthResponseDto> CreateTokensAndUpdateRefreshToken(string email, string role, string entityType)
    {
        var refreshToken = GenerateRefreshToken();
        var expires = DateTime.UtcNow.AddHours(1);
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        if (entityType == "User")
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = refreshTokenExpiry;
                await _dbcontext.SaveChangesAsync();
            }
        }
        else if (entityType == "Admin")
        {
            var admin = await _dbcontext.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin != null)
            {
                admin.RefreshToken = refreshToken;
                admin.RefreshTokenExpiryTime = refreshTokenExpiry;
                await _dbcontext.SaveChangesAsync();
            }
        }

        var jwtResponse = GenerateJwt(email, role);
        return new AuthResponseDto(jwtResponse.AccessToken, refreshToken, jwtResponse.ExpiresAt);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user != null && user.RefreshTokenExpiryTime > DateTime.UtcNow)
        {
            return await CreateTokensAndUpdateRefreshToken(user.Email, "User", "User");
        }

        var admin = await _dbcontext.Admins.FirstOrDefaultAsync(a => a.RefreshToken == refreshToken);
        if (admin != null && admin.RefreshTokenExpiryTime > DateTime.UtcNow)
        {
            return await CreateTokensAndUpdateRefreshToken(admin.Email, "Admin", "Admin");
        }

        throw new UnauthorizedAccessException("Invalid or expired refresh token");
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _dbcontext.SaveChangesAsync();
            return;
        }

        var admin = await _dbcontext.Admins.FirstOrDefaultAsync(a => a.RefreshToken == refreshToken);
        if (admin != null)
        {
            admin.RefreshToken = null;
            admin.RefreshTokenExpiryTime = null;
            await _dbcontext.SaveChangesAsync();
            return;
        }

        throw new InvalidOperationException("Invalid refresh token");
    }
}
