using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{
    public class Journal
    {
        public Guid UniqueID { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string IssueNumber { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string File { get; set; }
        public string Cover { get; set; }
        public List<string>? Keyword { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CategoryId { get; set; }
    }
}
