using System;
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.API.DTOs
{
    public class UserDto
    {
        

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string UserName { get; set; }
         [Required(ErrorMessage = "Username is required")]    
              public string Password { get; set;  }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        [RegularExpression("^(Pending|Approved|Rejected)$", ErrorMessage = "Approve status must be 'Pending', 'Approved', or 'Rejected'")]
        public string ApproveStatus { get; set; }

        public bool Status { get; set; }
    }
}