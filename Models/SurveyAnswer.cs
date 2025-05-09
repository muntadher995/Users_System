using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.Models
{
    public class SurveyAnswer
    {
        public Guid UniqueID { get; set; }
        public string? AnswerText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Keys
        public Guid? QuestionOptionId { get; set; }
        public QuestionOption? QuestionOption { get; set; }

        public Guid QuestionId { get; set; }
        public SurveyQuestion? Question { get; set; }
    }

}
