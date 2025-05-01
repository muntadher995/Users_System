using System.ComponentModel.DataAnnotations;

namespace UserSystem.Models
{
   public class User
{
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string UserName { get; set; } = default!;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = default!;

    [Required]
    public string PasswordHash { get; set; } = default!;

    public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
}

// Models/Admin.cs  – identical to User but kept separate per requirement
public class Admin
{
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string AdminName { get; set; } = default!;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = default!;

    [Required]
    public string PasswordHash { get; set; } = default!;

    public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
}

// Models/Role.cs
 

public class Role
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = default!;

    public ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
}

// Models/RoleAssignment.cs  – flexible join table (either UserId or AdminId populated)
public class RoleAssignment
{
    public int Id { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public Guid? AdminId { get; set; }
    public Admin? Admin { get; set; }
}

}
