// DTOs/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.API.DTOs;

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiry { get; set; }

    public AuthResponseDto(string accessToken, string refreshToken, DateTime accessTokenExpiry)
    {
        Success = true;
        Message = "successfully ";  
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        AccessTokenExpiry = accessTokenExpiry;
    }

    public AuthResponseDto()
    {
        Success = false;
        Message = "error  "; // رسالة الخطأ الافتراضية
    }
}



// DTOs/AuthResponseDto.cs
//public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);
