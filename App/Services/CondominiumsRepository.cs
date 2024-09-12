using NextCondoApi.Entity;

namespace NextCondoApi.Services;

public interface ICondominiumsRepository : IGenericRepository<Condominium> 
{
    public Task<List<Condominium>> GetByOwnerIdAsync(Guid ownerId);
}

public class CondominiumsRepository : GenericRepository<Condominium>, ICondominiumsRepository
{
    public CondominiumsRepository(
        NextCondoApiDbContext context, 
        ILogger<CondominiumsRepository> logger) 
        : base(context, logger)
    {
    }

    public async Task<List<Condominium>> GetByOwnerIdAsync(Guid ownerId)
    {
        List<Condominium> all = await GetAllAsync();
        return all.Where(condo => condo.OwnerId.Equals(ownerId)).ToList();
    }
}
