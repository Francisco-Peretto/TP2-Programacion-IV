using System.ComponentModel.DataAnnotations;

namespace TP2_Programacion_IV.Models.User.Dto;

public class LoginRequestDTO
{
    [Required, EmailAddress] 
    public string Email { get; set; } = default!;

    [Required, MinLength(6)] 
    public string Password { get; set; } = default!;
}
