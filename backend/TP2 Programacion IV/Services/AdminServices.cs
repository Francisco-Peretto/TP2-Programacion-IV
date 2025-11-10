using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

namespace TP2_Programming_IV.Services;

public class AdminServices
{
    private readonly AppDbContext _ctx;

    public AdminServices(AppDbContext ctx) => _ctx = ctx;

    public async Task<object> GetMetricsAsync()
    {
        var totalCourses = await _ctx.Courses.CountAsync();
        var totalUsers = await _ctx.Users.CountAsync();

        // No category/price/isActive anymore
        return new { totalCourses, totalUsers };
    }
}
