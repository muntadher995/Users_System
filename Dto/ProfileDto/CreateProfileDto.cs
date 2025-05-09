namespace Ai_LibraryApi.Dto.ProfileDto
{

    public class CreateProfileDto
    {
        public string? Address { get; set; }
        public string? Country { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Bio { get; set; }
        public string? Photo { get; set; }
        public Guid UserId { get; set; }
    }

}
