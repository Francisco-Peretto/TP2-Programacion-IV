using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using TP2_Programacion_IV.Models.User.Dto;
using TP2_Programacion_IV.Repositories;

namespace TP2_Programacion_IV.Services;

public class AuthServices
{
    private readonly UserRepository _users;
    private readonly RoleRepository _roles;
    private readonly IConfiguration _cfg;
    private readonly EncoderServices _enc;

    public AuthServices(UserRepository users, RoleRepository roles, IConfiguration cfg, EncoderServices enc)
    {
        _users = users;
        _roles = roles;
        _cfg = cfg;
        _enc = enc;
    }

    // ---------- LOGIN ----------
    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Email y contraseña son requeridos.");

        // Find user by email
        var user = await _users.GetByEmailAsync(dto.Email);
        if (user is null)
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        // Verify password
        if (!_enc.Verify(dto.Password, user.Password))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        // Take the first role (since UsuarioDTO expects a single string)
        var roleName = user.Roles.FirstOrDefault()?.Name ?? "User";

        // Create JWT token
        var token = GenerateJwtToken(user, roleName);

        // Build DTOs
        var usuarioDto = new UsuarioDTO(user.Id, user.Email, roleName);
        var response = new LoginResponseDTO(token, usuarioDto);

        return response;
    }

    // ---------- REGISTER ----------
    public async Task<UsuarioDTO> RegisterAsync(RegisterRequestDTO dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            throw new ArgumentException("Las contraseñas no coinciden.");

        var existing = await _users.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new InvalidOperationException("El email ya está registrado.");

        var hashed = _enc.Hash(dto.Password);

        var newUser = new User
        {
            Email = dto.Email,
            UserName = dto.UserName,
            Password = hashed
        };

        // Assign default "User" role if exists
        var defaultRole = await _roles.GetByIdAsync(2);
        if (defaultRole != null)
            newUser.Roles.Add(defaultRole);

        await _users.AddAsync(newUser);

        return new UsuarioDTO(newUser.Id, newUser.Email, defaultRole?.Name ?? "User");
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
