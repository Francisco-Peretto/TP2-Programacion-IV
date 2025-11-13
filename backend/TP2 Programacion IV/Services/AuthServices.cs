using Domain.Entities;
using TP2_Programming_IV.Models.User.Dto;
using TP2_Programming_IV.Repositories;

namespace TP2_Programming_IV.Services;

public class AuthServices
{
    private readonly UserRepository _users;
    private readonly RoleRepository _roles;
    private readonly IEncoderServices _encoder;

    public AuthServices(UserRepository users, RoleRepository roles, IEncoderServices encoder)
    {
        _users = users;
        _roles = roles;
        _encoder = encoder;
    }

    // ---------- VALIDATE USER (used by AuthController.Login) ----------
    public async Task<UserDTO?> ValidateUserAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Email y contraseña son requeridos.");

        var user = await _users.GetByEmailAsync(email);
        if (user is null)
            return null;

        if (!_encoder.Verify(password, user.Password))
            return null;

        var roleName = user.Role?.Name ?? "User";
        return new UserDTO(user.Id, user.UserName, user.Email, roleName);
    }

    // ---------- REGISTER ----------
    public async Task<UserDTO> RegisterAsync(RegisterRequestDTO dto)
    {
        var existing = await _users.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new InvalidOperationException("Email ya registrado.");

        var defaultRole = await _roles.GetByNameAsync("User")
                        ?? throw new InvalidOperationException("Rol 'User' no encontrado. ¿Se hizo el seed inicial?");

        var entity = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            Password = _encoder.Hash(dto.Password),
            RoleId = defaultRole.Id
        };

        await _users.CreateAsync(entity);

        return new UserDTO(
            entity.Id,
            entity.UserName,
            entity.Email,
            defaultRole.Name
        );
    }

    // --------- UPDATE USER ROLE ---------
    public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user is null) return false;

        var role = await _roles.GetByIdAsync(roleId);
        if (role is null) return false;

        user.RoleId = role.Id;
        await _users.UpdateAsync(user);
        return true;
    }
}
