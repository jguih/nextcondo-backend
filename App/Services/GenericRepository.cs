using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;

namespace NextCondoApi.Services;

public class GenericRepository<TEntity> 
    : IGenericRepository<TEntity> 
    where TEntity : BaseEntity
{
    protected NextCondoApiDbContext db;
    protected ILogger logger;
    protected DbSet<TEntity> entities;

    public GenericRepository(NextCondoApiDbContext context, ILogger<GenericRepository<TEntity>> logger)
    {
        this.db = context;
        this.logger = logger;
        this.entities = context.Set<TEntity>();
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await entities.AddAsync(entity);
        await db.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        db.Entry(entity).State = EntityState.Modified;
        await db.SaveChangesAsync();
    }

    public virtual async Task<bool> DeleteAsync(object id)
    {
        var existing = await entities.FindAsync(id);
        if (existing != null)
        {
            entities.Remove(existing);
            await db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id)
    {
        return await entities.FindAsync(id);
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await entities.AsNoTracking().ToListAsync();
    }
}
