
using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace UnitTests.Fakes;

internal class FakeUsersRepository : FakeGenericRepository<User>, IUsersRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        var all = await GetAllAsync();
        return all.Where(u => u.Email.Equals(email)).FirstOrDefault();
    }
}
