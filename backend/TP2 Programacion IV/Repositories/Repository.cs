using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class Repository<T> where T : class
{
    protected readonly AppDbContext _ctx;
    public Repository(AppDbContext ctx) => _ctx = ctx;

    public IQueryable<T> Query() => _ctx.Set<T>().AsQueryable();
    public async Task AddAsync(T entity) { _ctx.Set<T>().Add(entity); await _ctx.SaveChangesAsync(); }
    public async Task UpdateAsync(T entity) { _ctx.Set<T>().Update(entity); await _ctx.SaveChangesAsync(); }
    public async Task DeleteAsync(T entity) { _ctx.Set<T>().Remove(entity); await _ctx.SaveChangesAsync(); }
    public Task<T?> GetByIdAsync(params object[] key) => _ctx.Set<T>().FindAsync(key).AsTask();
}
