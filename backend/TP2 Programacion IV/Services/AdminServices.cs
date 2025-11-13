using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TP2_Programming_IV.Models.Admin.Dto;


namespace TP2_Programming_IV.Services;

public class AdminServices
{
    private readonly AppDbContext _ctx;

    public AdminServices(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    // ⬇️ add this method
    public async Task<AdminMetricsDTO> GetMetricsAsync()
    {
        // all courses
        var courses = await _ctx.Courses.ToListAsync();

        // enrollments grouped by course
        var enrollCounts = await _ctx.Enrollments
            .GroupBy(e => e.CourseId)
            .Select(g => new { CourseId = g.Key, Count = g.Count() })
            .ToListAsync();

        var enrollDict = enrollCounts.ToDictionary(x => x.CourseId, x => x.Count);

        // total courses
        var totalCourses = courses.Count;

        // total distinct students (anyone enrolled in at least one course)
        var totalStudents = await _ctx.Enrollments
            .Select(e => e.UserId)
            .Distinct()
            .CountAsync();

        var studentsPerCourse = courses
            .Select(c => new CourseStudentsDTO
            {
                CourseId = c.Id,
                CourseName = c.Name,   // ajusta si tu propiedad se llama distinto
                StudentCount = enrollDict.TryGetValue(c.Id, out var n) ? n : 0
            })
            .ToList();



        return new AdminMetricsDTO
        {
            TotalCourses = totalCourses,
            TotalStudents = totalStudents,
            StudentsPerCourse = studentsPerCourse
        };
    }

    public async Task EnrollStudentAsync(int userId, int courseId)
    {
        // aseguramos que el curso existe
        var courseExists = await _ctx.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            throw new ArgumentException($"Curso {courseId} no existe.");

        // aseguramos que el usuario existe
        var userExists = await _ctx.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new ArgumentException($"Usuario {userId} no existe.");

        // evitamos duplicados (gracias al índice único también)
        var alreadyEnrolled = await _ctx.Enrollments
            .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);

        if (alreadyEnrolled)
            return; // o lanzar excepción si prefieres

        var enrollment = new Enrollment
        {
            CourseId = courseId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _ctx.Enrollments.Add(enrollment);
        await _ctx.SaveChangesAsync();
    }

    public async Task UnenrollStudentAsync(int userId, int courseId)
    {
        var uc = await _ctx.UserCourses
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);

        if (uc != null)
        {
            _ctx.UserCourses.Remove(uc);
            await _ctx.SaveChangesAsync();
        }
    }


}
