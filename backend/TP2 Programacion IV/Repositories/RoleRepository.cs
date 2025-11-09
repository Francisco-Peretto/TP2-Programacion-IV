using Infrastructure.Data;
using Domain.Entities;


namespace TP2_Programacion_IV.Repositories;
public class RoleRepository : Repository<Role>
{
    public RoleRepository(AppDbContext ctx) : base(ctx) { }
}
