namespace Ai_LibraryApi.Dto.ProfileDto
{
    public class UpdateProfileDto
    {
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? Birthdate { get; set; }
        public string? Bio { get; set; }
         public string ApprovalStatus { get; set; } // حالة الموافقة
         public string? RejectionReason { get; set; }
        public IFormFile Cv_File { get; set; }

        public IFormFile? PhotoFile { get; set; }
    }
}
