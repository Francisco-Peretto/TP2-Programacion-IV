using System.ComponentModel.DataAnnotations;

namespace TP2_Programming_IV.Models.Course.Dto;

public class UpdateCourseDTO
{
    [Required, MaxLength(120)] 
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

}
