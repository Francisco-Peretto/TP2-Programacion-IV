using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP2_Programming_IV.Domain.Entities.UserCourse.Dto;
using TP2_Programming_IV.Models.Admin.Dto;
using TP2_Programming_IV.Services;

namespace TP2_Programming_IV.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // 
public class AdminController : ControllerBase
{
    private readonly AdminServices _admin;

    public class EnrollRequestDTO
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
    }

    public AdminController(AdminServices admin)
    {
        _admin = admin;
    }

    [HttpGet("metrics")]
    public async Task<ActionResult<AdminMetricsDTO>> GetMetrics()
    {
        var result = await _admin.GetMetricsAsync();
        return Ok(result);
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollRequestDTO dto)
    {
        await _admin.EnrollStudentAsync(dto.UserId, dto.CourseId);
        return NoContent(); // 204
    }

    [HttpDelete("enroll")]
    public async Task<IActionResult> UnenrollStudent([FromBody] EnrollRequestDTO dto)
    {
        await _admin.UnenrollStudentAsync(dto.UserId, dto.CourseId);
        return NoContent(); // 204
    }


}
