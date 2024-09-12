
namespace NextCondoApi.Services;

public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Get all entities for this repository.
    /// </summary>
    /// <returns>All entities.</returns>
    public Task<List<TEntity>> GetAllAsync();
    /// <summary>
    /// Get an entity by it's id.
    /// </summary>
    /// <param name="id">Entities' id.</param>
    /// <returns>The entity.</returns>
    public Task<TEntity?> GetByIdAsync(object id);
    /// <summary>
    /// Add a new entity.
    /// </summary>
    /// <param name="entity">Entity to be added</param>
    /// <returns>A boolean that indicated wether the operation was successfull or not.</returns>
    public Task AddAsync(TEntity entity);
    /// <summary>
    /// Delete an entity.
    /// </summary>
    /// <param name="id">Entities' id.</param>
    /// <returns>A boolean that indicated wether the operation was successfull or not.</returns>
    public Task<bool> DeleteAsync(object id);
    /// <summary>
    /// Save changes to the database.
    /// </summary>
    /// <returns>Number of rows affected</returns>
    public Task<int> SaveAsync();
}
