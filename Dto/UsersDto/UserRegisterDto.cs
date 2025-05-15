// DTOs/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.API.DTOs;




public class UserRegisterDto
{
    [Required(ErrorMessage = "UserName is required.")]
    [StringLength(100, ErrorMessage = "UserName cannot be longer than 100 characters.")]
    
    public string UserName{ get; set;}

    [Required]
    [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string email { get; set; }


    [Required(ErrorMessage = "phone is required.")]
    [Phone(ErrorMessage = "Phone number is not valid.")]
    public string phone { get; set; }



    [Required(ErrorMessage = "approve_status is required.")]
 
    [RegularExpression("^(Pending|Approved|Rejected)$", ErrorMessage = "Approve status must be 'Pending', 'Approved', or 'Rejected'")]

    public string? approve_status { get; set; }


    [Required(ErrorMessage = "status is required.")]
    public bool status { get; set;  } = false;
   

}


