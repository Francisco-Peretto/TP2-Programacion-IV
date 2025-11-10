using System.ComponentModel.DataAnnotations;

namespace TP2_Programming_IV.Models.User.Dto;

public class RegisterRequestDTO
{
    [Required, MaxLength(100)]
    public string UserName { get; set; } = default!;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = default!;

    [Required, MinLength(6)]
    public string Password { get; set; } = default!;

    [Required, MinLength(6)]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = default!;
}
