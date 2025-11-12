using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using TP2_Programming_IV.Models.User.Dto;   // ← usa un solo namespace de DTOs
using TP2_Programming_IV.Repositories;

namespace TP2_Programming_IV.Services;

public class AuthServices
{
    private readonly UserRepository _users;
    private readonly RoleRepository _roles;
    private readonly IConfiguration _cfg;
    private readonly IEncoderServices _encoder;

    public AuthServices(UserRepository users, RoleRepository roles, IEncoderServices encoder, IConfiguration cfg)
    {
        _users = users;
        _roles = roles;
        _encoder = encoder;
        _cfg = cfg;
    }

    // ---------- LOGIN ----------
    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Email y contraseña son requeridos.");

        var user = await _users.GetByEmailAsync(dto.Email);
        if (user is null)
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        if (!_encoder.Verify(dto.Password, user.Password))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var roleName = user.Role?.Name ?? "User";

        var token = GenerateJwtToken(user, roleName);

        var usuarioDto = new UserDTO(user.Id, user.UserName, user.Email, roleName);
        return new LoginResponseDTO(token, usuarioDto);
    }

    // ---------- REGISTER ----------
    public async Task<UserDTO> RegisterAsync(RegisterRequestDTO dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            throw new ArgumentException("Las contraseñas no coinciden.");

        var existing = await _users.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new InvalidOperationException("El email ya está registrado.");

        var hashed = _encoder.Hash(dto.Password);

        var newUser = new User
        {
            Email = dto.Email,
            UserName = dto.UserName,
            Password = hashed
        };

        // Rol por defecto: "User" (ej: id = 2)
        var defaultRole = await _roles.GetByIdAsync(2);
        if (defaultRole == null)
            throw new InvalidOperationException("No existe el rol por defecto 'User'.");

        // Como es 1 rol por usuario, asigna RoleId o la navegación:
        newUser.RoleId = defaultRole.Id; // preferible para evitar problemas de tracking

        await _users.AddAsync(newUser);

        return new UserDTO(
            newUser.Id,
            newUser.Email,
            newUser.UserName,
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

    // ----------- LOGOUT ----------
    public Task<bool> LogoutAsync(string? token)
    {
        // Option A: stateless JWT -> no server action needed.
        // Option B: implement a blacklist store and add the token.
        return Task.FromResult(true);
    }

    // ---------- HELPER: Generate JWT ----------
    private string GenerateJwtToken(User user, string roleName)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, roleName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
