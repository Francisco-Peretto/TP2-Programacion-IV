using Microsoft.EntityFrameworkCore;
using TP2_Programacion_IV.Models.Course;
using TP2_Programacion_IV.Models.Course.Dto;
using TP2_Programacion_IV.Repositories;
using TP2_Programacion_IV.Utils;
using Domain.Entities;


namespace TP2_Programacion_IV.Services;

public class CourseServices
{
    private readonly CourseRepository _repo;
    public CourseServices(CourseRepository repo) => _repo = repo;

    public async Task<PagedResult<CoursesDTO>> GetAllAsync(int page, int pageSize, string? q, string? category)
    {
        var query = _repo.Query();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c => c.Name.Contains(q) || (c.Description ?? "").Contains(q));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(c => c.Category == category);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => new CoursesDTO(c.Id, c.Name, c.Category, c.Price)).ToListAsync();

        return new PagedResult<CoursesDTO>(items, page, pageSize, total);
    }

    public Task<CourseDTO?> GetByIdAsync(int id) =>
        _repo.Query().Where(c => c.Id == id)
            .Select(c => new CourseDTO(c.Id, c.Name, c.Description, c.Category, c.Price, c.IsActive))
            .FirstOrDefaultAsync();

    public async Task<int> CreateAsync(CreateCourseDTO dto)
    {
        var e = new Course { Name = dto.Title, Description = dto.Description, Category = dto.Category, Price = dto.Price, IsActive = dto.IsActive };
        await _repo.AddAsync(e);
        return e.Id;
    }

    public async Task UpdateAsync(int id, UpdateCourseDTO dto)
    {
        var e = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Curso no encontrado");
        e.Name = dto.Title; e.Description = dto.Description; e.Category = dto.Category; e.Price = dto.Price; e.IsActive = dto.IsActive;
        await _repo.UpdateAsync(e);
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Curso no encontrado");
        await _repo.DeleteAsync(e);
    }
}
