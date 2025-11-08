using TP2_Programacion_IV.Config;
using TP2_Programacion_IV.Models.Role;

namespace TP2_Programacion_IV.Repositories;
public class RoleRepository : Repository<Role>
{
    public RoleRepository(ApplicationDbContext ctx) : base(ctx) { }
}
