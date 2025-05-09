using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{

    public class LibraryRequest
    {
        public Guid UniqueID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string File { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
        public List<string>? Keyword { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Keys
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public Guid UserId { get; set; }
       
    }
}
