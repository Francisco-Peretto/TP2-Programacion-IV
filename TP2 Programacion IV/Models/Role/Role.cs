using System.ComponentModel.DataAnnotations;

namespace TP2_Programacion_IV.Models.Role;

public class Role
{
    [Key]
    public int Id { get; set; }
    
    [Required, MaxLength(30)]
    public string Name { get; set; } = default!; // "Admin" | "User"
}
