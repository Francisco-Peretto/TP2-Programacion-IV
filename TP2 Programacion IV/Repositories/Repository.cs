using Microsoft.EntityFrameworkCore;
using TP2_Programacion_IV.Config;

namespace TP2_Programacion_IV.Repositories;

public class Repository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _ctx;
    protected readonly DbSet<TEntity> _db;

    public Repository(ApplicationDbContext ctx) { _ctx = ctx; _db = ctx.Set<TEntity>(); }

    public Task<List<TEntity>> GetAllAsync() => _db.ToListAsync();
    public Task<TEntity?> GetByIdAsync(int id) => _db.FindAsync(id).AsTask();
    public async Task AddAsync(TEntity e) { _db.Add(e); await _ctx.SaveChangesAsync(); }
    public async Task UpdateAsync(TEntity e) { _ctx.Entry(e).State = EntityState.Modified; await _ctx.SaveChangesAsync(); }
    public async Task DeleteAsync(TEntity e) { _db.Remove(e); await _ctx.SaveChangesAsync(); }
}
