using System.ComponentModel.DataAnnotations;

namespace TP2_Programacion_IV.Models.User.Dto;

public class CreateUserDTO
{
    [Required, EmailAddress] public string Email { get; set; } = default!;
    [Required, MinLength(6)] public string Password { get; set; } = default!;
    [Required] public string FullName { get; set; } = default!;
    [Required] public int RoleId { get; set; }
}
