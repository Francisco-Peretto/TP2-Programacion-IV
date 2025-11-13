using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP2_Programacion_IV.Models.User.Dto;
using TP2_Programming_IV.Models.User.Dto;
using TP2_Programming_IV.Services;

namespace TP2_Programming_IV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthServices _auth;
    public AuthController(AuthServices auth) => _auth = auth;

    [HttpGet("whoami")]
    [Authorize]
    public IActionResult WhoAmI() => Ok(User.Claims.Select(c => new { c.Type, c.Value }));


    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO dto)
    {
        try { return Ok(await _auth.LoginAsync(dto)); }
        catch (UnauthorizedAccessException e) { return Unauthorized(new { error = e.Message }); }
    }

    [HttpPost("logout")]
    [Authorize] // user must be authenticated to "logout"
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers.Authorization.ToString();
        var ok = await _auth.LogoutAsync(token);
        return ok ? Ok(new { message = "Logged out" }) : BadRequest();
    }

    // PUT /api/auth/{id}/roles
    [HttpPut("{id:int}/roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDTO dto)
    {
        var ok = await _auth.UpdateUserRoleAsync(id, dto.RoleId);
        return ok ? NoContent() : NotFound();
    }

    // POST /api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
    {
        var created = await _auth.RegisterAsync(dto);
        // Return 201 + minimal info (no password ever)
        return CreatedAtAction(nameof(Register), new { id = created.Id }, created);
    }
}
