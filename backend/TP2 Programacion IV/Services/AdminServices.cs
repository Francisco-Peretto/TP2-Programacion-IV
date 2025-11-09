using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace TP2_Programacion_IV.Services;

public class AdminServices
{
    private readonly AppDbContext _ctx;
    public AdminServices(AppDbContext ctx) => _ctx = ctx;

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
