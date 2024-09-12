
using NextCondoApi.Entity;
using NextCondoApi.Services;
using TestFakes;

namespace UnitTests.Fakes;

public class FakeRolesRepository : FakeGenericRepository<Role>, IRolesRepository
{
    private FakeRolesRepository()
    {    
    }

    public static async Task<FakeRolesRepository> Create()
    {
        var repository = new FakeRolesRepository();
        var defaultRole = FakeRolesFactory.GetDefault();
        await repository.AddAsync(defaultRole);
        return repository;
    }

    public async Task<Role> GetDefaultAsync()
    {
        Role? defaultRole = await GetByIdAsync(FakeRolesFactory.GetDefault().Name);
        ArgumentNullException.ThrowIfNull(defaultRole, nameof(defaultRole));
        return defaultRole;
    }
}