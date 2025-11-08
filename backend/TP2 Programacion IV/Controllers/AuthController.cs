using Microsoft.AspNetCore.Mvc;
using TP2_Programacion_IV.Models.User.Dto;
using TP2_Programacion_IV.Services;

namespace TP2_Programacion_IV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthServices _auth;
    public AuthController(AuthServices auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO dto)
    {
        try { return Ok(await _auth.LoginAsync(dto)); }
        catch (UnauthorizedAccessException e) { return Unauthorized(new { error = e.Message }); }
    }
}
