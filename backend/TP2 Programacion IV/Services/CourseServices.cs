using Microsoft.EntityFrameworkCore;
using TP2_Programming_IV.Models.Course;
using TP2_Programming_IV.Models.Course.Dto;
using TP2_Programming_IV.Repositories;
using TP2_Programming_IV.Utils;
using Domain.Entities;

namespace TP2_Programming_IV.Services;

public class CourseServices
{
    private readonly CourseRepository _repo;
    public CourseServices(CourseRepository repo) => _repo = repo;

    // Removed `category` since the entity no longer has it
    public async Task<PagedResult<CoursesDTO>> GetAllAsync(int page, int pageSize, string? q)
    {
        var query = _repo.Query();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c =>
                c.Name.Contains(q) ||
                (c.Description ?? string.Empty).Contains(q));

        var total = await query.CountAsync();

        // Order by Id to keep deterministic paging without CreatedAt
        var items = await query
            .OrderByDescending(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            // Adapt to your simplified list DTO (Id, Name)
            .Select(c => new CoursesDTO(c.Id, c.Name, c.Description))
            .ToListAsync();

        return new PagedResult<CoursesDTO>(items, page, pageSize, total);
    }

    public Task<CourseDTO?> GetByIdAsync(int id) =>
        _repo.Query()
            .Where(c => c.Id == id)
            // Adapt to your detailed DTO (Id, Name, Description)
            .Select(c => new CourseDTO(c.Id, c.Name, c.Description))
            .FirstOrDefaultAsync();

    public async Task<int> CreateAsync(CreateCourseDTO dto)
    {
        var e = new Course
        {
            // Keeping Title from your DTO shape
            Name = dto.Title,
            Description = dto.Description
        };

        await _repo.AddAsync(e);
        return e.Id;
    }

    public async Task UpdateAsync(int id, UpdateCourseDTO dto)
    {
        var e = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Course not found");
        e.Name = dto.Title;
        e.Description = dto.Description;

        await _repo.UpdateAsync(e);
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Course not found");
        await _repo.DeleteAsync(e);
    }
}
