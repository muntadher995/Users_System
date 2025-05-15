using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.Models
{

    public class RoleAssignment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = default!;

        public Guid? UserId { get; set; }
        public User? User { get; set; }

        public Guid? AdminId { get; set; }
        public Admin? Admin { get; set; }
    }
}
