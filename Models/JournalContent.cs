using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{
    public class JournalContent
    {
        public Guid UniqueID { get; set; }
        public string Title { get; set; }
        public string PFrom { get; set; }
        public string PTo { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string File { get; set; }
        public string? Cover { get; set; }
        public List<string>? Keyword { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid JournalId { get; set; }
    }

}
