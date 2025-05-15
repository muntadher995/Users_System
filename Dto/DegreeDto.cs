namespace Ai_LibraryApi.Dto
{
    public class DegreeDto
    {
        public Guid UniqueID { get; set; }
        public string? DegreeName { get; set; }
        public string? University { get; set; }
        public string? Specialization { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid ProfileId { get; set; }
    }

    public class CreateDegreeDto
    {
        public string? DegreeName { get; set; }
        public string? University { get; set; }
        public string? Specialization { get; set; }
        public Guid ProfileId { get; set; }
    }

    public class UpdateDegreeDto
    {
        public string? DegreeName { get; set; }
        public string? University { get; set; }
        public string? Specialization { get; set; }
    }
}