using System.ComponentModel.DataAnnotations;

namespace TP2_Programacion_IV.Models.Course;

public class Course
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    [Required, MaxLength(50)]
    public string Category { get; set; } = "General";

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
