using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{
    public class Degree
    {
        public Guid UniqueID { get; set; }
        public string? DegreeName { get; set; }
        public string? University { get; set; }
        public string? Specialization { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Key
        public Guid ProfileId { get; set; }
        public Profile? Profile { get; set; }
    }


}
