namespace Domain.UserCourse;

public class UserCourse
{
    public int UserId { get; set; }
    public Entities.User User { get; set; } = null!;

    public int CourseId { get; set; }
    public Entities.Course Course { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}
