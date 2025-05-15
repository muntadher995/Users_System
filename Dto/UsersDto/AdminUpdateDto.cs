// DTOs/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.API.DTOs;


public class AdminUpdateDto
{
     
    [StringLength(100, ErrorMessage = "AdminName cannot be longer than 100 characters.")]
 

    public string? AdminName { get; set; }
 
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    public IFormFile? photo { get; set; }

    public bool? isActive { get; set; }
  
    public string? Password { get; set; }
    
    public bool status { get; set; } 


}