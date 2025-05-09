using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.Models
{
    public class SurveyQuestion
    {
        public Guid UniqueID { get; set; }
        public string Question { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public int Order { get; set; }
        public bool Private { get; set; } 
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Key
        public Guid SurveyId { get; set; }
        public Survey? Survey { get; set; }

        // Navigation properties
        public ICollection<QuestionOption>? Options { get; set; }
        public ICollection<SurveyAnswer>? Answers { get; set; }
    }
}
