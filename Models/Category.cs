using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{
    public class Category
    {
        public Guid UniqueID { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Navigation property
        public ICollection<LibraryRequest>? LibraryRequests { get; set; }
    }
}
