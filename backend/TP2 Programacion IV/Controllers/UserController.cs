// In the Web API project
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP2_Programacion_IV.Services;
using TP2_Programacion_IV.Models.User.Dto;

namespace TP2_Programacion_IV.Controllers;

[ApiController]
[Route("api/[controller]")]            // -> api/User (because class is UserController)
public class UserController : ControllerBase
{
    private readonly UserServices _svc;
    public UserController(UserServices svc) => _svc = svc;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Get(int id) =>
        (await _svc.GetByIdAsync(id)) is { } u ? Ok(u) : NotFound();

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateUserDTO dto) =>
        CreatedAtAction(nameof(Get), new { id = (await _svc.CreateAsync(dto)).Id }, null);

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateUserDTO dto) =>
        await _svc.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        await _svc.DeleteAsync(id) ? NoContent() : NotFound();
}
