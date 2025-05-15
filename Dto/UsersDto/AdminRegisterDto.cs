// DTOs/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.API.DTOs;


public class AdminRegisterDto
{
    [Required(ErrorMessage = "AdminName is required.")]
    [StringLength(100, ErrorMessage = "AdminName cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "AdminName can only contain letters and numbers.")]

    public string AdminName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters.")]
    public string Password { get; set; }

    public IFormFile? photo {  get; set; }

    [Required]  
    public bool? isActive { get; set; }

    


}