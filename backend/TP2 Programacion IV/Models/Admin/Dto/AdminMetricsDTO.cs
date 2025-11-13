namespace TP2_Programming_IV.Models.Admin.Dto
{
    public class AdminMetricsDTO
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }

        public List<CourseStudentsDTO> StudentsPerCourse { get; set; } = new();
    }
}
