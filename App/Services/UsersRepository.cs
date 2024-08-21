using NextCondoApi.Entity;

namespace NextCondoApi.Services;

public interface IUsersRepository : IGenericRepository<User>
{
    public Task<User?> GetByEmailAsync(string email);
};

public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    public UsersRepository(
        NextCondoApiDbContext context,
        ILogger<UsersRepository> logger)
        : base(context, logger)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var users = await GetAllAsync();
        if (users.Count != 0)
        {
            var byEmail = users.Where((user) => user.Email == email);
            if (!byEmail.Any())
            {
                return null;
            }
            return byEmail.First();
        }
        return null;
    }
}
