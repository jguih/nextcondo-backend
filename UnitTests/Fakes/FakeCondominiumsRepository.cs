using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace UnitTests.Fakes;

class FakeCondominiumsRepository : FakeGenericRepository<Condominium>, ICondominiumsRepository
{
    public async Task<List<Condominium>> GetByOwnerIdAsync(Guid ownerId)
    {
        List<Condominium> all = await GetAllAsync();
        return all.Where(condo => condo.OwnerId.Equals(ownerId)).ToList();
    }
}
