using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using TP2_Programacion_IV.Models.User.Dto;
using TP2_Programming_IV.Models.User.Dto;
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
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Email y contraseña son requeridos.");

        // Find user by email
        var user = await _users.GetByEmailAsync(dto.Email);
        if (user is null)
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        // Verify password
        if (!_encoder.Verify(dto.Password, user.Password))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        // Take the first role (since UsuarioDTO expects a single string)
        var roleName = user.Roles.FirstOrDefault()?.Name ?? "User";

        // Create JWT token
        var token = GenerateJwtToken(user, roleName);

        // Build DTOs
        var usuarioDto = new UserDTO(user.Id, user.UserName, user.Email, user.RoleName);
        var response = new LoginResponseDTO(token, usuarioDto);

        return response;
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

        // Assign default "User" role if exists
        var defaultRole = await _roles.GetByIdAsync(2);
        if (defaultRole != null)
            newUser.Roles.Add(defaultRole);

        await _users.AddAsync(newUser);

        return new UserDTO(
             newUser.Id,
             newUser.Email,
             newUser.UserName,
             defaultRole?.Name ?? "User"
         );

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
