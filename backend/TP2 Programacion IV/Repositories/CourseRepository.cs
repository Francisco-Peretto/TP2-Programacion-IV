using Microsoft.EntityFrameworkCore;
using TP2_Programacion_IV.Config;
using TP2_Programacion_IV.Models.Course;

namespace TP2_Programacion_IV.Repositories;

public class CourseRepository : Repository<Course>
{
    public CourseRepository(ApplicationDbContext ctx) : base(ctx) { }
    public IQueryable<Course> Query() => _db.AsNoTracking();
}
