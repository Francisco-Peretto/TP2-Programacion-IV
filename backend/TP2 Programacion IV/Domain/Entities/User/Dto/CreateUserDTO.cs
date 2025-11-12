namespace TP2_Programming_IV.Models.User.Dto;

public class CreateUserDTO
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    // Optional — you can set default role = 2 ("User") if not provided
    public int RoleId { get; set; } = 2;
}
