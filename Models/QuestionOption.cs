using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.Models
{
    public class QuestionOption
    {
        public Guid UniqueID { get; set; }
        public string Text { get; set; }
        public string? Value { get; set; }
        public int Order { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Key
        public Guid QuestionId { get; set; }
        public SurveyQuestion? Question { get; set; }
    }
}
