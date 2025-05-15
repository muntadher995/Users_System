// DTOs/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.API.DTOs;




public class UserUpdateDto
{
     
    [StringLength(100, ErrorMessage = "UserName cannot be longer than 100 characters.")]
    
    public string? UserName{ get; set;}

    
    [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters.")]
    public string? Password { get; set; }

   
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? email { get; set; }


    
    [Phone(ErrorMessage = "Phone number is not valid.")]
    public string? phone { get; set; }

   
    public string? approve_status { get; set; }


    
    public bool? status { get; set;  } = false;
   

}


