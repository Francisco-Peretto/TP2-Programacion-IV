using System.ComponentModel.DataAnnotations;

namespace TP2_Programacion_IV.Models.Role.Dto;

public class CreateRoleDTO
{
    [Required] public string Name { get; set; } = default!;
}
