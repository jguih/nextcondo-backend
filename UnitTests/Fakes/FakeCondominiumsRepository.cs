using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace UnitTests.Fakes;

class FakeCondominiumsRepository : FakeGenericRepository<Condominium>, ICondominiumsRepository
{
    public async Task<List<Condominium>> GetByOwnerIdAsync(Guid ownerId)
    {
        await Task.Delay(1);
        var condominiums = Entities
            .Where(c => c.OwnerId.Equals(ownerId))
            .ToList();
        return condominiums;
    }

    public override async Task<Condominium?> GetByIdAsync(object id)
    {
        await Task.Delay(1);
        var condo = Entities.Find(e => e.Id.Equals(id));
        return condo;
    }
}
