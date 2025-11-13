using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    // GET /api/auth/whoami
    [HttpGet("whoami")]
    [AllowAnonymous]   // ⬅️ TEMPORARY for debugging
    public IActionResult WhoAmI()
    {
        var identity = User?.Identity;

        var result = new
        {
            IsAuthenticated = identity?.IsAuthenticated ?? false,
            Name = identity?.Name,
            AuthType = identity?.AuthenticationType,
            Claims = User?.Claims.Select(c => new { c.Type, c.Value }).ToList()
        };

        return Ok(result);
    }

    // POST /api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        try
        {
            var user = await _auth.ValidateUserAsync(dto.Email, dto.Password);
            if (user == null)
                return Unauthorized(new { error = "Invalid credentials" });

            // Create claims for the cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.RoleName ?? "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Issue the cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true, // Keep logged in across browser sessions
                    ExpiresUtc = DateTime.UtcNow.AddHours(2)
                });

            return Ok(new
            {
                message = "Login successful",
                user.Email,
                user.RoleName
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out successfully" });
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
        return CreatedAtAction(nameof(Register), new { id = created.Id }, created);
    }
}
