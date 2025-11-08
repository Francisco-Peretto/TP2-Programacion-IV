using Microsoft.EntityFrameworkCore;
using TP2_Programacion_IV.Models.Course;
using TP2_Programacion_IV.Models.User;
using TP2_Programacion_IV.Models.Role;

namespace TP2_Programacion_IV.Config;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}
