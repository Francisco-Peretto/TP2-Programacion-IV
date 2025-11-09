using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class UserRepository : Repository<User>
{
    public UserRepository(AppDbContext ctx) : base(ctx) { }

    public Task<User?> GetByEmailAsync(string email) =>
        _ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
}
