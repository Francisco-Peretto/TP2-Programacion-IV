namespace TP2_Programacion_IV.Models.Course.Dto;

public record CourseDTO(
    int Id,
    string Title,
    string? Description,
    string Category,
    decimal Price,
    bool IsActive
);
