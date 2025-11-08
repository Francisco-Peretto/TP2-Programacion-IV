using Microsoft.EntityFrameworkCore;
using TP2_Programacion_IV.Config;
using TP2_Programacion_IV.Models.User;

namespace TP2_Programacion_IV.Repositories;

public class UserRepository : Repository<User>
{
    public UserRepository(ApplicationDbContext ctx) : base(ctx) { }
    public Task<User?> GetByEmailAsync(string email) =>
        _ctx.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
}
