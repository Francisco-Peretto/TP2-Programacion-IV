using Microsoft.EntityFrameworkCore;
using TP2_Programacion_IV.Config;

namespace TP2_Programacion_IV.Services;

public class AdminServices
{
    private readonly ApplicationDbContext _ctx;
    public AdminServices(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<object> GetMetricsAsync()
    {
        var totalCourses = await _ctx.Courses.CountAsync();
        var totalUsers = await _ctx.Users.CountAsync();
        var byCategory = await _ctx.Courses
            .GroupBy(c => c.Category)
            .Select(g => new { category = g.Key, count = g.Count() })
            .ToListAsync();

        return new { totalCourses, totalUsers, byCategory };
    }
}
