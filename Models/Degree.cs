using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{
    public class Degree
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
