using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ai_LibraryApi.Models
{
    public class Profile
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public Guid ID { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? Birthdate { get; set; }
        public string? Bio { get; set; }
        public string? Photo { get; set; }
        public string Cv_File { get; set; }
        public string ApprovalStatus { get; set; } = "Pending"; // القيم المحتملة: "Pending", "Approved", "Rejected"
        public string? RejectionReason { get; set; } // سبب الرفض (إذا تم رفض الملف الشخصي)

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Key
        public Guid UserId { get; set; }

        // Navigation property
        public User? User { get; set; } 
        public ICollection<Degree>? Degrees { get; set; }
    }
}
