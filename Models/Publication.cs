using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{
    public class Publication
    {
        public Guid UniqueID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? Cover { get; set; }
        public string? File { get; set; }
        public List<string>? Keyword { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
