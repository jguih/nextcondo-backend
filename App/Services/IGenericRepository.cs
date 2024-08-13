namespace NextCondoApi.Services;

public interface IGenericRepository<TEntity> where TEntity : class
{
    public Task<List<TEntity>> GetAllAsync();
    public Task<TEntity?> GetByIdAsync(object id);
    public Task<bool> AddAsync(TEntity entity);
    public Task<bool> DeleteAsync(object id);
    public Task SaveAsync();
}
