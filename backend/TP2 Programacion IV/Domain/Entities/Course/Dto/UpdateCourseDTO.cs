using System.ComponentModel.DataAnnotations;

namespace TP2_Programacion_IV.Models.Course.Dto;

public class UpdateCourseDTO
{
    [Required, MaxLength(120)] 
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    [Required, MaxLength(50)] 
    public string Category { get; set; } = default!;

    [Range(0, double.MaxValue)] 
    public decimal Price { get; set; }

    public bool IsActive { get; set; }
}
