namespace Ai_LibraryApi.Dto.ProfileDto
{
    
        public class ProfileDto
        {
            public Guid UniqueID { get; set; }
            public string? Address { get; set; }
            public string? Country { get; set; }
            public DateTime? Birthdate { get; set; }
            public string? Bio { get; set; }
            public string? Photo { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public Guid UserId { get; set; }
        }

      
    
}
