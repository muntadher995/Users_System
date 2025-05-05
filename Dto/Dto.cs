// DTOs/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace UserSystem.API.DTOs;



public record RegisterDto(
    [Required, MaxLength(100)] string Name,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);

public record UserRegisterDto(
    [Required, MaxLength(100)] string UserName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);

// DTOs/AdminRegisterDto.cs  (same idea; you may merge if desired)
public record AdminRegisterDto(
    [Required, MaxLength(100)] string AdminName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);

// DTOs/LoginDto.cs
public record LoginDto(
    [Required, MaxLength(100)] string Name,
    [Required] string Password);

// DTOs/AuthResponseDto.cs
public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);
