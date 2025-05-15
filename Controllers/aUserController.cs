 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ai_LibraryApi.API.DTOs;
using Ai_LibraryApi.API.Services;
using Microsoft.Extensions.Logging;
using Ai_LibraryApi.Helper;
using Microsoft.EntityFrameworkCore;
namespace Ai_LibraryApi.API.Controllers;


 [Route("api/[controller]")]

 [ApiController]


public class a_UserController : ControllerBase
{
  private readonly IAuthService _auth;
    private readonly ILogger<a_UserController> _logger;
    private readonly Ai_LibraryApiDbContext _dbContext;

    public a_UserController(IAuthService auth, ILogger<a_UserController> logger, Ai_LibraryApiDbContext dbContext)
    {
        _auth = auth;
        _logger = logger;
        _dbContext = dbContext;
    }


    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto dto)
    {
        try
        {
            
            var result = await _auth.RegisterUserAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering user");
            return StatusCode(500, new { Message = ex.Message });
        }
    }
 
    [HttpPost("login/user")]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDto dto)
    {
        try
        {
            
            var result = await _auth.LoginUserAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging in user");
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



    [Authorize(Roles = "Admin")]
 


    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetAllUsers(string? search, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var query = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var loweredSearch = search.Trim().ToLower();
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.Trim().ToLower().Contains(loweredSearch)) ||
                    (u.Email != null && u.Email.Trim().ToLower().Contains(loweredSearch))
                );
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    
                    UserName = u.UserName,
                    Email = u.Email,
                    Phone = u.phone,
                    ApproveStatus = u.approve_status,
                    Status = u.status
                })
                .ToListAsync();

            var result = new PagedResult<UserDto>(users, totalCount, pageNumber, pageSize);
            return Ok(new ApiResponse<PagedResult<UserDto>>(true, "Users retrieved successfully", result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, new ApiResponse<PagedResult<UserDto>>(false, "Internal server error", null));
        }
    }

    [Authorize(Roles = "Admin")]

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(Guid id)
    {
        try
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new ApiResponse<UserDto>(false, "User not found", null));
            }

            var userDto = new UserDto
            {
            
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.phone,
                ApproveStatus = user.approve_status,
                Status = user.status
            };

            return Ok(new ApiResponse<UserDto>(true, "User retrieved successfully", userDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user");
            return StatusCode(500, new ApiResponse<UserDto>(false, "Internal server error", null));
        }
    }

    [Authorize(Roles = "Admin")]

    [HttpPut]
 
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto,Guid id)
    {
        try
        {
             
            var result = await _auth.UpdateUserAsync(dto,id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user");
            return StatusCode(500, new { Message = ex.Message });
        }
    }


    [Authorize(Roles = "Admin")]

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(Guid id)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(id);
            
            if (user == null)
            {
                return NotFound(new ApiResponse<object>(false, "User not found", null));
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new ApiResponse<object>(true, "User deleted successfully", null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
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