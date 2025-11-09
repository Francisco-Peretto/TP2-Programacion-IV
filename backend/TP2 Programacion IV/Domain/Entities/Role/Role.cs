// Domain/Entities/Rol.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}
    