
using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace UnitTests.Fakes;

internal class FakeUsersRepository : FakeGenericRepository<User>, IUsersRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        await Task.Delay(1);
        var user = Entities
            .Where(u => u.Email.Equals(email))
            .FirstOrDefault();
        return user;
    }

    public override async Task<User?> GetByIdAsync(object id)
    {
        await Task.Delay(1);
        var user = Entities.Find(e => e.Id.Equals(id));
        return user;
    }
}
