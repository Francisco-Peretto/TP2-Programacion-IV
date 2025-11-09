namespace Domain.Entities
{
    public class UserCourse
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    }
}
