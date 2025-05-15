using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ai_LibraryApi.API.DTOs;
using Ai_LibraryApi.API.Services;
using Microsoft.Extensions.Logging;
using Ai_LibraryApi.Helper;
using Ai_LibraryApi.Services;
using Microsoft.AspNetCore.Identity;


namespace Ai_LibraryApi.API.Controllers;

 [Route("api/[controller]")]
[ApiController]

public class AdminController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ILogger<AdminController> _logger;
    private readonly Ai_LibraryApiDbContext _dbContext;
    public AdminController(IAuthService authService, Ai_LibraryApiDbContext dbContext)
    {
        _auth  = authService;
        _dbContext = dbContext;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] AdminRegisterDto dto)
    {
        try
        {
            var result = await _auth.RegisterAdminAsync(dto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = ex.Message });
        }
    }
 
    [HttpPost("login/admin")]
    public async Task<IActionResult> LoginAdmin([FromBody] AdminLoginDto dto)
    {
        try
        {
          
            var result = await _auth.LoginAdminAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging in admin");
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
           
            var result = await _auth.RefreshTokenAsync(refreshToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while refreshing token");
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PagedResult<AdminDto>>>> GetAllAdmins(string? search, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var query = _dbContext.Admins.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var loweredSearch = search.Trim().ToLower();
                query = query.Where(a =>
                    (a.AdminName != null && a.AdminName.Trim().ToLower().Contains(loweredSearch)) ||
                    (a.Email != null && a.Email.Trim().ToLower().Contains(loweredSearch))
                );
            }

            var totalCount = await query.CountAsync();

            var admins = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AdminDto
                {
                    Id = a.Id,
                    AdminName = a.AdminName,
                    Email = a.Email,
                    IsActive = a.isActive,
                    // Dynamically construct full file path for photo
                    Photo = string.IsNullOrEmpty(a.photo) ? null : $"{Request.Scheme}://{Request.Host}/uploads/admins/{a.photo}"
                })
                .ToListAsync();

            var result = new PagedResult<AdminDto>(admins, totalCount, pageNumber, pageSize);
            return Ok(new ApiResponse<PagedResult<AdminDto>>(true, "Admins retrieved successfully", result));
        }
        catch (Exception ex)
        {
            // If you have a logger, you can log the error
            // _logger.LogError(ex, "Error retrieving admins");
            return StatusCode(500, new ApiResponse<PagedResult<AdminDto>>(false, "Internal server error", null));
        }
    }
    [HttpGet("{id}")]
   [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<AdminDto>>> GetAdminById(Guid id)
    {
        try
        {
            var admin = await _dbContext.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (admin == null)
            {
                return NotFound(new ApiResponse<AdminDto>(false, "Admin not found", null));
            }

            var adminDto = new AdminDto
            {
                Id = admin.Id,
                AdminName = admin.AdminName,
                Email = admin.Email,
                IsActive = admin.isActive,
                // Dynamically construct full file path for photo
                Photo = string.IsNullOrEmpty(admin.photo) ? null : $"{Request.Scheme}://{Request.Host}/uploads/admins/{admin.photo}"
            };

            return Ok(new ApiResponse<AdminDto>(true, "Admin retrieved successfully", adminDto));
        }
        catch (Exception ex)
        {
            // If you have a logger, you can log the error
            // _logger.LogError(ex, "Error retrieving admin");
            return StatusCode(500, new ApiResponse<AdminDto>(false, "Internal server error", null));
        }
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateAdmin(Guid id, [FromForm] AdminUpdateDto dto)
    {
        try
        {
            var result = await _auth.UpdateAdminAsync(dto, id);

            if (result.Success)
            {
                return Ok(new ApiResponse<object>(true, result.Message, null));
            }

            return BadRequest(new ApiResponse<object>(false, result.Message, null));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>(false, "Internal server error: " + ex.Message, null));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAdmin(Guid id)
    {
        try
        {
            var result = await _auth.DeleteAdminAsync(id);

            if (result.Success)
            {
                return Ok(new ApiResponse<object>(true, result.Message, null));
            }

            return BadRequest(new ApiResponse<object>(false, result.Message, null));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>(false, "Internal server error: " + ex.Message, null));
        }
    }
}
























/*// Controllers/AuthController.cs
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


    *//*
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

    *//*


     
    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody]  UserRegisterDto dto)
        => Ok(await _auth.RegisterUserAsync(dto));


     
    [Authorize(Roles = "Admin")]

    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterDto dto)
        => Ok(await _auth.RegisterAdminAsync(dto));



    

    [HttpPost("login/user")]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDto dto)
        => Ok(await _auth.LoginUserAsync(dto));


    [Authorize(Roles = "Admin")]
    [HttpPost("login/admin")]
    public async Task<IActionResult> LoginAdmin([FromBody] AdminLoginDto dto)
        => Ok(await _auth.LoginAdminAsync(dto));


     
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        => Ok(await _auth.RefreshTokenAsync(refreshToken));

   *//* [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
    {
        await _auth.RevokeRefreshTokenAsync(refreshToken);
        return Ok(new { message = "Token revoked successfully" });
    }*//*
}
 
*/