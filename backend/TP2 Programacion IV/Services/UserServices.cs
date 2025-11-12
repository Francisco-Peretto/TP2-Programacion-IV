using Domain.Entities;
using TP2_Programming_IV.Models.User.Dto;
using TP2_Programming_IV.Repositories;
using TP2_Programming_IV.Services; // for IEncoderServices if you use it

namespace TP2_Programming_IV.Services;

public class UserServices
{
    private readonly UserRepository _users;
    private readonly RoleRepository _roles;
    private readonly IEncoderServices _encoder;

    public UserServices(UserRepository users, RoleRepository roles, IEncoderServices encoder)
    {
        _users = users;
        _roles = roles;
        _encoder = encoder;
    }

    // ---------- CREATE USER ----------
    public async Task<UserDTO> CreateAsync(CreateUserDTO dto)
    {
        // Optional: validate uniqueness
        var existing = await _users.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new InvalidOperationException("El email ya está registrado.");

        // Hash password
        var hashed = _encoder.Hash(dto.Password);

        // Create entity instance
        var userEntity = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            Password = hashed,
            RoleId = dto.RoleId   // or assign a default role
        };

        // Persist to DB
        await _users.AddAsync(userEntity);

        // Get role name if available
        var roleName = (await _roles.GetByIdAsync(userEntity.RoleId))?.Name ?? "User";

        // Return DTO
        return new UserDTO(
            userEntity.Id,
            userEntity.Email,
            userEntity.UserName,
            roleName
        );
    }

    // ---------- GET ALL USERS ----------
    public async Task<IEnumerable<UserDTO>> GetAllAsync()
    {
        var users = await _users.GetAllAsync();

        return users.Select(u => new UserDTO(
            u.Id,
            u.Email,
            u.UserName,
            u.Role?.Name ?? "User"
        ));
    }

    // ---------- GET USER BY ID ----------
    public async Task<UserDTO?> GetByIdAsync(int id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null) return null;

        return new UserDTO(
            user.Id,
            user.Email,
            user.UserName,
            user.Role?.Name ?? "User"
        );
    }

    // ---------- UPDATE ----------
    public async Task UpdateAsync(UpdateUserDTO dto)
    {
        var user = await _users.GetByIdAsync(dto.Id)
                   ?? throw new InvalidOperationException("Usuario no encontrado.");

        user.UserName = dto.UserName;
        user.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.Password = _encoder.Hash(dto.Password);
        user.RoleId = dto.RoleId;

        await _users.UpdateAsync(user);
    }

    // ---------- DELETE ----------
    public async Task<bool> DeleteAsync(int id)
    {
        return await _users.DeleteAsync(id);
    }
}
