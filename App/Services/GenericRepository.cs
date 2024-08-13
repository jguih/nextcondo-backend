using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;

namespace NextCondoApi.Services;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected NextCondoApiDbContext db;
    protected ILogger logger;
    protected DbSet<TEntity> entitiesDb;

    public GenericRepository(NextCondoApiDbContext context, ILogger<GenericRepository<TEntity>> logger)
    {
        this.db = context;
        this.logger = logger;
        this.entitiesDb = context.Set<TEntity>();
    }

    public virtual async Task<bool> AddAsync(TEntity entity)
    {
        try
        {
            await entitiesDb.AddAsync(entity);
            return true;
        } catch (Exception ex)
        {
            logger.LogError(ex, "Error adding entity");
            return false;
        }
    }

    public virtual async Task<bool> DeleteAsync(object id)
    {

        try
        {
            var existing = await entitiesDb.FindAsync(id);
            if (existing != null)
            {
                entitiesDb.Remove(existing);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting entity {name} with id {id}", nameof(TEntity),id);
            return false;
        }
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        try
        {
            return await entitiesDb.ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all entities");
            return [];
        }
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id)
    {
        try
        {
            return await entitiesDb.FindAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting entity with {id}", id);
            return null;
        }
    }

    public virtual async Task SaveAsync()
    {
        await db.SaveChangesAsync();
    }
}
