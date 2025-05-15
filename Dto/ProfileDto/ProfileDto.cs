namespace Ai_LibraryApi.Dto.ProfileDto
{
    
        public class ProfileDto
        {
            public Guid UniqueID { get; set; }
            public string? Address { get; set; }
            public string? Country { get; set; }
            public string? Birthdate { get; set; }
            public string? Bio { get; set; }
            public string? Photo { get; set; }
            public string Cv_File { get; set; }
            public string ApprovalStatus { get; set; } // حالة الموافقة
            public string? RejectionReason { get; set; } // سبب الرفض (إذا وجد)

            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public Guid UserId { get; set; }
        }

      
    
}
