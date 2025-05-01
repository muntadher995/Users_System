// Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserSystem.API.DTOs;
using UserSystem.API.Services;

namespace UserSystem.API.Controllers;

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


    /*[Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
        => Ok(await _auth.RegisterAsync(dto));*/

    [Authorize(Roles = "Admin")]
    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser(UserRegisterDto dto)
        => Ok(await _auth.RegisterUserAsync(dto));
    [Authorize(Roles = "Admin")]
    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterAdmin(AdminRegisterDto dto)
        => Ok(await _auth.RegisterAdminAsync(dto));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
        => Ok(await _auth.LoginAsync(dto));
}
 
