using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TP2_Programacion_IV.Models.User.Dto;
using TP2_Programacion_IV.Repositories;

namespace TP2_Programacion_IV.Services;

public class AuthServices
{
    private readonly UserRepository _users;
    private readonly IConfiguration _cfg;
    private readonly EncoderServices _enc;

    public AuthServices(UserRepository users, IConfiguration cfg, EncoderServices enc)
    { _users = users; _cfg = cfg; _enc = enc; }

    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto)
    {
        var user = await _users.GetByEmailAsync(dto.Email);
        if (user is null || !_enc.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"], audience: _cfg["Jwt:Audience"],
            claims: claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: creds);

        return new LoginResponseDTO(new JwtSecurityTokenHandler().WriteToken(token),
            new UsuarioDTO(user.Id, user.Email, user.Role.Name));
    }
}
