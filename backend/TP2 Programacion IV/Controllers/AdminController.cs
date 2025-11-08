using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP2_Programacion_IV.Services;

namespace TP2_Programacion_IV.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AdminServices _svc;
    public AdminController(AdminServices svc) => _svc = svc;

    [HttpGet("metrics")]
    public async Task<ActionResult<object>> GetMetrics() => Ok(await _svc.GetMetricsAsync());
}
