using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{
  
    public class Survey
    {
        public Guid UniqueID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Cover { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Key
        public Guid CreatedBy { get; set; } // admin/user ID

        // Navigation property
        public ICollection<SurveyQuestion>? Questions { get; set; }
    }
}
