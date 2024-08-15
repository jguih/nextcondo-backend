﻿using Microsoft.EntityFrameworkCore;
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
        await entitiesDb.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> DeleteAsync(object id)
    {
        var existing = await entitiesDb.FindAsync(id);
        if (existing != null)
        {
            entitiesDb.Remove(existing);
            return true;
        }
        return false;
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await entitiesDb.ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id)
    {
        return await entitiesDb.FindAsync(id);
    }

    public virtual async Task<int> SaveAsync()
    {
        return await db.SaveChangesAsync();
    }
}