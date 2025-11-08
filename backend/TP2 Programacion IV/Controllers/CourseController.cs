using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP2_Programacion_IV.Models.Course.Dto;
using TP2_Programacion_IV.Services;
using TP2_Programacion_IV.Utils;

namespace TP2_Programacion_IV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly CourseServices _svc;
    public CourseController(CourseServices svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<PagedResult<CoursesDTO>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? q = null, [FromQuery] string? category = null)
        => Ok(await _svc.GetAllAsync(page, pageSize, q, category));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDTO>> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } dto ? Ok(dto) : NotFound();

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult> Create([FromBody] CreateCourseDTO dto)
    {
        var id = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateCourseDTO dto)
    {
        await _svc.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
