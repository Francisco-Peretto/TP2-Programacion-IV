namespace TP2_Programming_IV.Models.User.Dto;

public class UpdateUserDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public int RoleId { get; set; }
}
