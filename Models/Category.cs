using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public Guid UniqueID { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }
        public bool Status { get; set; }=false;
        public DateTime CreatedAt { get; set; }= DateTime.Now;  
        public DateTime UpdatedAt { get; set; }=DateTime.Now;
 
        
    }
}
