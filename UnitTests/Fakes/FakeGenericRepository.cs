using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace UnitTests.Fakes;

public class FakeGenericRepository<TEntity> : IGenericRepository<TEntity> 
    where TEntity : BaseEntity
{
    protected readonly List<TEntity> Entities = [];

    public virtual async Task AddAsync(TEntity entity)
    {
        await Task.Delay(1);
        Entities.Add(entity);
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        await Task.Delay(1);
        for (int i = 0; i < Entities.Count; i++)
        {
            var item = Entities[i];
            if (item.GetId().Equals(entity.GetId()))
            {
                Entities[i] = entity;
            }
        }
    }

    public virtual async Task<bool> DeleteAsync(object id)
    {
        await Task.Delay(1);
        var existing = await GetByIdAsync(id);
        if (existing != null)
        {
            Entities.Remove(existing);
            return true;
        }
        return false;
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id)
    {
        await Task.Delay(1);
        throw new NotImplementedException();
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        await Task.Delay(1);
        return Entities;
    }
}