// DTOs/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.API.DTOs;




public class UserLoginDto
{

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string UserName { get; set; }
 [Required(ErrorMessage = "Username is required")]
    public string Password { get; set;  }


}
