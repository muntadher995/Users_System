namespace Ai_LibraryApi.Dto.ProfileDto
{
    public class ProfileApprovalDto
    {
        public string ApprovalStatus { get; set; } // "Approved" أو "Rejected"
        public string? RejectionReason { get; set; } // مطلوب فقط إذا كانت الحالة "Rejected"
    }
}