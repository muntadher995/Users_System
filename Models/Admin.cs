using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.Models
{
    public class Admin
    {
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string AdminName { get; set; } = default!;

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = default!;

        public string? photo { get; set; } = default!;

        [Required]
        public string PasswordHash { get; set; } = default!;
        public string? RefreshTokenHashed { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool isActive { get; set; } = false;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
        
    }
}
