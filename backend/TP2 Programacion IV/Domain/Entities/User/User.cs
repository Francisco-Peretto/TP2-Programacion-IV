using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string UserName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    // 🔹 Foreign key to Role
    [Required]
    public int RoleId { get; set; }

    // 🔹 Navigation property
    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }

    // 🔹 Many-to-many (User ↔ Course)
    public ICollection<Domain.UserCourse.UserCourse> UserCourses { get; set; } = new List<Domain.UserCourse.UserCourse>();
}
