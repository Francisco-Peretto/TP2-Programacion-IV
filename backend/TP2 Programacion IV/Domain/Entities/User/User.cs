// Domain/Entities/Usuario.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP2_Programacion_IV.Models.Role;

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

    // NOTE: AuthServices expects "Password"
    [Required]
    public string Password { get; set; } = null!;

    // Many-to-many to roles
    public ICollection<Role> Roles { get; set; } = new List<Role>();

    // Many-to-many to courses (usuario <-> course)
    public ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();


}