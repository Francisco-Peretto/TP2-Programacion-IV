namespace TP2_Programming_IV.Models.Admin.Dto
{
    public class CourseStudentsDTO
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
    }
}
