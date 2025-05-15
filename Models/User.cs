using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ai_LibraryApi.Models
{
   public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string? UserName { get; set; }

    public string? approve_status { get; set; }  

    [Required, EmailAddress  ]
    public string? Email { get; set; }
    public DateTime? emailVerifedAt { get; set; }
    public bool status { get; set; } = false;   

    [Required]
    public string PasswordHash { get; set; }
    public string? phone { get; set; }

    public DateTime? phoneVerifedAt { get; set; }
    public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
    public ICollection<Profile> Profiles { get; set; } = new List<Profile>();
     

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }

}
