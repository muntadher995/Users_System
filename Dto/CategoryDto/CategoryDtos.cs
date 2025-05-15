

using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.DTOs
{
    public class CategoryDto
    {
        public Guid UniqueID { get; set; }
        public string? Title { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } 
    }

    public class CreateCategoryDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public bool Status { get; set; }=false;
    }

    public class UpdateCategoryDto
    {
        public string? Title { get; set; }
        public bool? Status { get; set; }=false ;
    }

    public class CategorySearchParams
    {
        public string Title { get; set; }
        public bool Status { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}