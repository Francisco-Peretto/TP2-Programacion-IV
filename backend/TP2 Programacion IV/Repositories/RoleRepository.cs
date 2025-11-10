using Infrastructure.Data;
using Domain.Entities;


namespace TP2_Programming_IV.Repositories;
public class RoleRepository : Repository<Role>
{
    public RoleRepository(AppDbContext ctx) : base(ctx) { }
}
