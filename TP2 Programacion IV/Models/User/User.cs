using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP2_Programacion_IV.Models.Role;

namespace TP2_Programacion_IV.Models.User;

public class User
{
    [Key]
    public int Id { get; set; }


    [Required, MaxLength(150), EmailAddress]
    public string Email { get; set; } = default!;


    [Required]
    public string PasswordHash { get; set; } = default!;


    [Required]
    public int RoleId { get; set; }


    [ForeignKey(nameof(RoleId))]
    public Role.Role Role { get; set; } = default!;
}
