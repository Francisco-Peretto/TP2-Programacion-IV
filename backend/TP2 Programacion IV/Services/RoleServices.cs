using TP2_Programming_IV.Repositories;
using TP2_Programacion_IV.Models.Role.Dto;
using Domain.Entities;

namespace TP2_Programacion_IV.Services;

public class RoleServices
{
    private readonly RoleRepository _roles;

    public RoleServices(RoleRepository roles)
    {
        _roles = roles;
    }

    // ✅ Get all roles
    public async Task<IEnumerable<RoleDTO>> GetAllAsync()
    {
        var list = await _roles.GetAllAsync();
        return list.Select(r => new RoleDTO(r.Id, r.Name));
    }

    // ✅ Get a role by id
    public async Task<RoleDTO?> GetByIdAsync(int id)
    {
        var r = await _roles.GetByIdAsync(id);
        return r is null ? null : new RoleDTO(r.Id, r.Name);
    }

    // ✅ Create new role
    public async Task<RoleDTO> CreateAsync(CreateRoleDTO dto)
    {
        var entity = new Role { Name = dto.Name };
        await _roles.CreateAsync(entity);
        return new RoleDTO(entity.Id, entity.Name);
    }

    // ✅ Update existing role
    public async Task<bool> UpdateAsync(int id, UpdateRoleDTO dto)
    {
        var entity = await _roles.GetByIdAsync(id);
        if (entity is null) return false;

        entity.Name = dto.Name;
        await _roles.UpdateAsync(entity);
        return true;
    }

    // ✅ Delete role
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _roles.GetByIdAsync(id);
        if (entity is null) return false;

        await _roles.DeleteAsync(entity);
        return true;
    }
}
