﻿
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
 
using Ai_LibraryApi.API.DTOs;
using Ai_LibraryApi.Models;
using Ai_LibraryApi.Services;
namespace Ai_LibraryApi.API.Services;


public interface IAuthService
{
    Task<AuthResponseDto> RegisterUserAsync(UserRegisterDto dto);
    Task<AuthResponseDto> RegisterAdminAsync(AdminRegisterDto dto);
    Task<AuthResponseDto> LoginUserAsync(UserLoginDto dto);
    Task<AuthResponseDto> LoginAdminAsync(AdminLoginDto dto);
    Task<AuthResponseDto> UpdateUserAsync(UserUpdateDto dto, Guid id);
    Task<AuthResponseDto> UpdateAdminAsync(AdminUpdateDto dto, Guid id);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task<AuthResponseDto> DeleteAdminAsync(Guid id);
}


public class AuthService : IAuthService
{
    private readonly Ai_LibraryApiDbContext _dbcontext;
    private readonly IConfiguration _cfg;
    private readonly PasswordHasher<object> _hasher = new();
    private readonly FileService _fileService;

    public AuthService(Ai_LibraryApiDbContext ctx, IConfiguration cfg, FileService fileService)
    {
        _dbcontext = ctx;
        _cfg = cfg;
        _fileService = fileService;
    }
    public async Task<AuthResponseDto> RegisterUserAsync(UserRegisterDto dto)
    {
        if (await _dbcontext.Users.AnyAsync(u => u.UserName == dto.UserName))
            throw new InvalidOperationException("UserName already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            Email = dto.email,
            PasswordHash = _hasher.HashPassword(null!, dto.Password),
            phone=dto.phone,
            approve_status=dto.approve_status,
            status=dto.status,

        };

        _dbcontext.Users.Add(user);
        await _dbcontext.SaveChangesAsync();

        var userRole = await GetRoleIdByNameAsync("User");
        _dbcontext.RoleAssignments.Add(new RoleAssignment { UserId = user.Id, RoleId = userRole });
        await _dbcontext.SaveChangesAsync();

        return await CreateTokensAndUpdateRefreshToken(user.Email, "User", "User");
    }




    public async Task<AuthResponseDto> RegisterAdminAsync(AdminRegisterDto dto)
    {
        try
        {
            if (await _dbcontext.Admins.AnyAsync(a => a.AdminName == dto.AdminName))
                throw new InvalidOperationException("Email already in use.");

            string photoFileName = null;
            if (dto.photo != null)
            {
                photoFileName = await _fileService.SaveImageAsync(dto.photo, "admins");
            }

            var admin = new Admin
            {
                Id = Guid.NewGuid(),
                AdminName = dto.AdminName,
                Email = dto.Email,
                PasswordHash = _hasher.HashPassword(null!, dto.Password),
                photo = photoFileName,
                isActive = (bool)dto.isActive,
            };

            _dbcontext.Admins.Add(admin);
            await _dbcontext.SaveChangesAsync();

            var adminRole = await GetRoleIdByNameAsync("Admin");
            _dbcontext.RoleAssignments.Add(new RoleAssignment { AdminId = admin.Id, RoleId = adminRole });
            await _dbcontext.SaveChangesAsync();

            return await CreateTokensAndUpdateRefreshToken(admin.Email, "Admin", "Admin");
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = ex.Message,
            };
        }
    }









    /* public async Task<AuthResponseDto> RegisterAdminAsync(AdminRegisterDto dto)
     {
         if (await _dbcontext.Admins.AnyAsync(a => a.Email == dto.Email))
             throw new InvalidOperationException("Email already in use.");

         var admin = new Admin
         {
             Id = Guid.NewGuid(),
             AdminName = dto.AdminName,
             Email = dto.Email,
             PasswordHash = _hasher.HashPassword(null!, dto.Password),
             photo=dto.photo,

         };

         _dbcontext.Admins.Add(admin);
         await _dbcontext.SaveChangesAsync();

         var adminRole = await GetRoleIdByNameAsync("Admin");
         _dbcontext.RoleAssignments.Add(new RoleAssignment { AdminId = admin.Id, RoleId = adminRole });
         await _dbcontext.SaveChangesAsync();

         return await CreateTokensAndUpdateRefreshToken(admin.Email, "Admin", "Admin");
     }
 */
    public async Task<AuthResponseDto> LoginUserAsync(UserLoginDto dto)
    {
        try
        {
            var user = await _dbcontext.Users
                .Include(u => u.RoleAssignments).ThenInclude(ra => ra.Role)
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if (user != null && _hasher.VerifyHashedPassword(null!, user.PasswordHash, dto.Password) == PasswordVerificationResult.Success)
            {
                var role = user.RoleAssignments.FirstOrDefault()?.Role?.Name;
                if (role != "User")
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "  error"
                    };
                }

                return await CreateTokensAndUpdateRefreshToken(user.Email, role, "User");
            }

            return new AuthResponseDto
            {
                Success = false,
                Message = " ."
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = ex.Message
            };
        }
    }









    public async Task<AuthResponseDto> LoginAdminAsync(AdminLoginDto dto)
    {
        try
        {
            var admin = await _dbcontext.Admins
                .Include(a => a.RoleAssignments).ThenInclude(ra => ra.Role)
                .FirstOrDefaultAsync(a => a.AdminName == dto.AdminName);

            if (admin != null && _hasher.VerifyHashedPassword(null!, admin.PasswordHash, dto.Password) == PasswordVerificationResult.Success)
            {
                var role = admin.RoleAssignments.FirstOrDefault()?.Role?.Name;
                if (role != "Admin")
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "error "
                    };
                }

                return await CreateTokensAndUpdateRefreshToken(admin.Email, role, "Admin");
            }

            return new AuthResponseDto
            {
                Success = false,
                Message = " ."
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = ex.Message,
            };
        }
    }


    private string GenerateJwt(string email, string role, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(ClaimTypes.Role, role)
            },
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes);
    }




    private async Task<AuthResponseDto> CreateTokensAndUpdateRefreshToken(string email, string role, string entityType)
    {
        var refreshToken = GenerateRefreshToken();
        var hashedRefreshToken = _hasher.HashPassword(null!, refreshToken);
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        var accessTokenExpiry = DateTime.UtcNow.AddMinutes(15);

        if (entityType == "User")
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.RefreshTokenHash = hashedRefreshToken;
                user.RefreshTokenExpiryTime = refreshTokenExpiry;
                await _dbcontext.SaveChangesAsync();
            }
        }
        else if (entityType == "Admin")
        {
            var admin = await _dbcontext.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin != null)
            {
                admin.RefreshTokenHashed = hashedRefreshToken;
                admin.RefreshTokenExpiryTime = refreshTokenExpiry;
                await _dbcontext.SaveChangesAsync();
            }
        }

        var accessToken = GenerateJwt(email, role, accessTokenExpiry);
        return new AuthResponseDto(accessToken, refreshToken, accessTokenExpiry);


    }


    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var users = await _dbcontext.Users
            .Where(u => u.RefreshTokenExpiryTime > DateTime.UtcNow)
            .ToListAsync();

        foreach (var user in users)
        {
            if (_hasher.VerifyHashedPassword(null!, user.RefreshTokenHash, refreshToken) == PasswordVerificationResult.Success)
            {
                return await CreateTokensAndUpdateRefreshToken(user.Email, "User", "User");
            }
        }

        var admins = await _dbcontext.Admins
            .Where(a => a.RefreshTokenExpiryTime > DateTime.UtcNow)
            .ToListAsync();

        foreach (var admin in admins)
        {
            if (_hasher.VerifyHashedPassword(null!, admin.RefreshTokenHashed, refreshToken) == PasswordVerificationResult.Success)
            {
                return await CreateTokensAndUpdateRefreshToken(admin.Email, "Admin", "Admin");
            }
        }

        throw new UnauthorizedAccessException("Invalid or expired refresh token");
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {/*
        var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.RefreshTokenHash == refreshToken);
        if (user != null)
        {
            user.RefreshTokenHash = null;
            user.RefreshTokenExpiryTime = null;
            await _dbcontext.SaveChangesAsync();
            return;
        }*/

        var admin = await _dbcontext.Admins.FirstOrDefaultAsync(a => a.RefreshTokenHashed == refreshToken);
        if (admin != null)
        {
            admin.RefreshTokenHashed = null;
            admin.RefreshTokenExpiryTime = null;
            await _dbcontext.SaveChangesAsync();
            return;
        }

        throw new InvalidOperationException("Invalid refresh token");
    }

    private async Task<int> GetRoleIdByNameAsync(string roleName)
    {
        var role = await _dbcontext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null) throw new InvalidOperationException($"Role '{roleName}' not found.");
        return role.Id;
    }

    public async Task<AuthResponseDto> UpdateUserAsync(UserUpdateDto dto, Guid id)
    {
        try
        {

            var exuser = await _dbcontext.Users.FindAsync(id);
            if (exuser == null)
                throw new InvalidOperationException("User not found.");

            if (await _dbcontext.Users.AnyAsync(u => u.UserName == dto.UserName))
                throw new InvalidOperationException("Username already in use.");

            exuser.UserName = dto.UserName ?? exuser.UserName;

            exuser.Email = dto.email ?? exuser.Email;


            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                exuser.PasswordHash = _hasher.HashPassword(null!, dto.Password);
            }

            exuser.phone = dto.phone ?? exuser.phone;

 
            exuser.approve_status = dto.approve_status ?? exuser.approve_status;

            if (dto.status.HasValue)
            {
                exuser.status = dto.status.Value;
            }
           // exuser.status = dto.status.HasValue ? dto.status.Value : exuser.status;

            _dbcontext.Users.Update(exuser);


            await _dbcontext.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "User updated successfully.",


            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error updating user: " + ex.Message
            };
        }
    }

    public async Task<AuthResponseDto> UpdateAdminAsync(AdminUpdateDto dto, Guid id)
    {
        try
        {
            var exAdmin = await _dbcontext.Admins.FindAsync(id);
            if (exAdmin == null)
                throw new InvalidOperationException("Admin not found.");

            // Check if username is being changed and if it's already in use
            if (dto.AdminName != null && dto.AdminName != exAdmin.AdminName &&
                await _dbcontext.Admins.AnyAsync(a => a.AdminName == dto.AdminName))
                throw new InvalidOperationException("Admin name already in use.");



            // Update other properties
            exAdmin.AdminName = dto.AdminName ?? exAdmin.AdminName;
            exAdmin.Email = dto.Email ?? exAdmin.Email;

            // Update photo if provided
            if (dto.photo != null)
            {
                string photoFileName = await _fileService.SaveImageAsync(dto.photo, "admins", exAdmin.photo);
                exAdmin.photo = photoFileName ?? exAdmin.photo;
            }
            // Only update password if provided
            if (!string.IsNullOrEmpty(dto.Password))
            {
                exAdmin.PasswordHash = _hasher.HashPassword(null!, dto.Password) ?? exAdmin.PasswordHash;
            }

            // Update isActive status if provided
            if (dto.isActive.HasValue)
            {
                exAdmin.isActive = dto.isActive.Value;
            }

            _dbcontext.Admins.Update(exAdmin);
            await _dbcontext.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Admin updated successfully."
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error updating admin: " + ex.Message
            };
        }
    }


    public async Task<AuthResponseDto> DeleteAdminAsync(Guid id)
    {
        try
        {
            var admin = await _dbcontext.Admins
                .Include(a => a.RoleAssignments)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (admin == null)
                throw new InvalidOperationException("Admin not found.");

            // Remove related RoleAssignments first
            if (admin.RoleAssignments != null && admin.RoleAssignments.Any())
            {
                _dbcontext.RoleAssignments.RemoveRange(admin.RoleAssignments);
            }

            // Then remove the Admin
            _dbcontext.Admins.Remove(admin);
            await _dbcontext.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Admin removed successfully."
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error removing Admin: " + ex.Message
            };
        }
    }

}
/*    public async Task<AuthResponseDto> UpdateAdminAsync(AdminUpdateDto dto, Guid id)
    {
        try
        {

            var exAdmin = await _dbcontext.Admins.FindAsync(id);
            if (exAdmin == null)
                throw new InvalidOperationException("Admin not found.");


            exAdmin.AdminName = dto.AdminName ?? exAdmin.AdminName;

            exAdmin.Email = dto.Email ?? exAdmin.Email;

            exAdmin.PasswordHash = _hasher.HashPassword(null!, dto.Password) ?? exAdmin.PasswordHash;




             _dbcontext.Admins.Update(exAdmin);


            await _dbcontext.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Admin updated successfully.",


            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Error updating user: " + ex.Message
            };
        }
    }
*/









