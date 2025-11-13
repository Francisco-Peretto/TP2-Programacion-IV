namespace TP2_Programming_IV.Models.User.Dto;

public class UserDTO
{
    public int Id { get; }
    public string Email { get; }
    public string UserName { get; }
    public string RoleName { get; }

    public UserDTO(int id, string email, string userName, string roleName)
    {
        Id = id;
        Email = email;
        UserName = userName;
        RoleName = roleName;
    }
}
