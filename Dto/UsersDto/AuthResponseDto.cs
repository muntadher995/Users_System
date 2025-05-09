// DTOs/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.API.DTOs;




// DTOs/AuthResponseDto.cs
public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);
