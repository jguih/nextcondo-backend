using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace UnitTests.Fakes;

public class FakeGenericRepository<TEntity> : IGenericRepository<TEntity> 
    where TEntity : BaseEntity
{
    protected readonly List<TEntity> Entities = [];

    public async Task AddAsync(TEntity entity)
    {
        await Task.Delay(1);
        Entities.Add(entity);
    }

    public async Task<bool> DeleteAsync(object id)
    {
        await Task.Delay(1);
        var existing = Entities.Find(e => e.GetId()?.Equals(id) ?? false);
        if (existing != null)
        {
            Entities.Remove(existing);
            return true;
        }
        return false;
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        await Task.Delay(1);
        return Entities;
    }

    public async Task<TEntity?> GetByIdAsync(object id)
    {
        await Task.Delay(1);
        var existing = Entities.Find(e => e.GetId()?.Equals(id) ?? false);
        return existing;
    }

    public async Task<int> SaveAsync()
    {
        await Task.Delay(1);
        return 1;
    }
}