// Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ai_LibraryApi.API.DTOs;
using Ai_LibraryApi.API.Services;

namespace Ai_LibraryApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;



    [Authorize(Roles = "Admin")]
    [HttpGet("Admin-dashboard")]
    public IActionResult Dashboard()
    {
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        return Ok(new { role });
    }


    [Authorize(Roles = "User")]
    [HttpGet("User")]
    public IActionResult Profile()
    {

        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;



        return Ok(new { Message = "Only authenticated Users can see this./n",role });

    }


    //[Authorize(Roles = "Admin")]
    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody]  UserRegisterDto dto)
        => Ok(await _auth.RegisterUserAsync(dto));



    //[Authorize(Roles = "Admin")]
    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterDto dto)
        => Ok(await _auth.RegisterAdminAsync(dto));



    [HttpPost("login/user")]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDto dto)
        => Ok(await _auth.LoginUserAsync(dto));



    [HttpPost("login/admin")]
    public async Task<IActionResult> LoginAdmin([FromBody] AdminLoginDto dto)
        => Ok(await _auth.LoginAdminAsync(dto));

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        => Ok(await _auth.RefreshTokenAsync(refreshToken));

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
    {
        await _auth.RevokeRefreshTokenAsync(refreshToken);
        return Ok(new { message = "Token revoked successfully" });
    }
}
 
