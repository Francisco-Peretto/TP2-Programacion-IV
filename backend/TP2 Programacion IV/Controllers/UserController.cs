using Microsoft.AspNetCore.Mvc;
using TP2_Programming_IV.Services;
using TP2_Programming_IV.Models.User.Dto;

namespace TP2_Programming_IV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserServices _users;

    public UserController(UserServices users)
    {
        _users = users;
    }

    // GET: api/user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
    {
        var result = await _users.GetAllAsync();
        return Ok(result);
    }

    // GET: api/user/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDTO>> GetById(int id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    // POST: api/user
    [HttpPost]
    public async Task<ActionResult<UserDTO>> Create([FromBody] CreateUserDTO dto)
    {
        var created = await _users.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/user/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO dto)
    {
        if (id != dto.Id) return BadRequest("El id de la ruta no coincide con el del cuerpo.");
        await _users.UpdateAsync(dto);
        return NoContent();
    }

    // DELETE: api/user/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _users.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
