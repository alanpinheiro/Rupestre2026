using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _db;
    protected DbSet<T> Set => _db.Set<T>();

    protected BaseRepository(ApplicationDbContext db) => _db = db;

    public virtual async Task<T?> GetByIdAsync(int id)
        => await Set.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await Set.ToListAsync();

    public virtual Task<int> InsertAsync(T entity) => throw new NotImplementedException();
    public virtual Task UpdateAsync(T entity) => throw new NotImplementedException();

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await Set.FindAsync(id);
        if (entity is null) return;
        _db.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
