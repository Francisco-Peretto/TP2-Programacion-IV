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
        // total cursos
        var totalCourses = await _ctx.Courses.CountAsync();

        // total estudiantes (distintos) según las inscripciones
        var totalStudents = await _ctx.Enrollments
            .Select(e => e.UserId)
            .Distinct()
            .CountAsync();

        // estudiantes por curso
        var studentsPerCourse = await _ctx.Courses
            .Select(c => new CourseStudentsDTO
            {
                CourseId = c.Id,
                CourseName = c.Name,
                StudentCount = _ctx.Enrollments.Count(e => e.CourseId == c.Id)
            })
            .ToListAsync();

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

    public async Task<bool> UnenrollStudentAsync(int userId, int courseId)
    {
        var enrollment = await _ctx.Enrollments
            .SingleOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment == null)
            return false;   // no vínculo user–course

        _ctx.Enrollments.Remove(enrollment);
        await _ctx.SaveChangesAsync();
        return true;
    }



}
