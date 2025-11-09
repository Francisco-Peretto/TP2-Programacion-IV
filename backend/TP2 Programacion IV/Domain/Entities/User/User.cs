// Domain/Entities/Usuario.cs
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

    // Many-to-many to roles
    public ICollection<Role> Roles { get; set; } = new List<Role>();

    // Many-to-many to courses (usuario <-> course)
    public ICollection<Domain.UserCourse.UserCourse> UserCourses { get; set; } = new List<Domain.UserCourse.UserCourse>();
}