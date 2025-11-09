using Infrastructure.Data;
using Domain.Entities;

public class CourseRepository : Repository<Course>
{
    public CourseRepository(AppDbContext ctx) : base(ctx) { }
}
