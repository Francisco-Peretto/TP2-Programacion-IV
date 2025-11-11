using Domain.Entities;
using TP2_Programacion_IV.Models.User.Dto;
using TP2_Programming_IV.Repositories;

namespace TP2_Programacion_IV.Services;

public class UserServices
{
    private readonly UserRepository _users;
    private readonly RoleRepository _roles;

    public UserServices(UserRepository users, RoleRepository roles)
    {
        _users = users;
        _roles = roles;
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync()
    {
        var list = await _users.GetAllAsync();
        return list.Select(u => new UserDTO(
            u.Id,
            u.Email,
            u.UserName,
            u.Role?.Name ?? "User"
        ));
    }

    public async Task<UserDTO?> GetByIdAsync(int id)
    {
        var u = await _users.GetByIdAsync(id);
        if (u is null) return null;

        return new UserDTO(
            u.Id,
            u.Email,
            u.UserName,
            u.Role?.Name ?? "User"
        );
    }


    // ✅ Create new user
    public async Task<UserDTO> CreateAsync(CreateUserDTO dto)
    {
        var role = await _roles.GetByIdAsync(dto.RoleId);
        if (role is null)
            throw new ArgumentException("Invalid role ID");

        var entity = new User
        {
            Email = dto.Email,
            UserName = dto.FullName,
            Password = dto.Password,   // ⚠️ Hash this if you have an encoder
            Id = dto.RoleId
        };

        await _users.CreateAsync(entity);

        return new UserDTO(
            entity.Id,
            entity.Email,
            entity.UserName,
            role.Name
        );
    }

    // Update existing user
    public async Task<bool> UpdateAsync(int id, UpdateUserDTO dto)
    {
        var u = await _users.GetByIdAsync(id);
        if (u is null) return false;

        if (!string.IsNullOrWhiteSpace(dto.Email))
            u.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.FullName))
            u.UserName = dto.FullName;
        if (dto.RoleId.HasValue)
            u.Id = dto.RoleId.Value;

        await _users.UpdateAsync(u);
        return true;
    }

    // Delete user
    public Task<bool> DeleteAsync(int id) => _users.DeleteAsync(id);
}
